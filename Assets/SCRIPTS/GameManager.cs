using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameSignals;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }
    private GameState currentGameState = GameState.Calibrating;

    [Header("Match")]
    [SerializeField] private bool autoStartAfterBothTutorials = true;
    [SerializeField] float changeToPlayingStateDuration = 1f;
    [SerializeField] float startCountdown = 3f;
    [SerializeField] private float matchLengthSeconds = 60f;
    [SerializeField] bool isCountdownAllowed = false;
    [SerializeField] Text StartCountdownText;
    [SerializeField] Text GameCountdownText;

    [Header("Input")]
    [SerializeField] private string verticalInputName = "Vertical";

    [Header("Players")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [Header("UI Panels (split 50/50)")]
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;

    public Player Player1 => player1;
    public Player Player2 => player2;

    private readonly HashSet<int> _calibDone = new();
    private readonly HashSet<int> _tutoDone = new();
    private float _matchTimer;
    private bool _running;

    private void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        CalibrationDone += OnCalibrationDone;
        GameStateChanged += (GameState gameState) => currentGameState = gameState;
    }

    private void OnDisable()
    {
        CalibrationDone -= OnCalibrationDone;
        GameStateChanged -= (GameState gameState) => currentGameState = gameState;
    }

    private void Start()
    {
        _running = false;
        _matchTimer = matchLengthSeconds;

        RaisePlayerSideAssigned(0, PlayerSide.Left);
        RaisePlayerSideAssigned(1, PlayerSide.Right);

        GameCountdownText.transform.parent.gameObject.SetActive(false);
        StartCountdownText.gameObject.SetActive(false);

        RaiseCalibrationStarted();
        RaiseGameState(GameState.Calibrating);
    }

    private void Update()
    {
        switch (currentGameState)   
        {
            case GameState.Calibrating:
                if (!player1.selected)
                {
                    if (InputManager.Instance.IsUpPressed(verticalInputName, "0"))
                    {
                        RaisePlayerSelected(0);
                    }
                }

                if (!player2.selected)
                {
                    if (InputManager.Instance.IsUpPressed(verticalInputName, "1"))
                    {
                        RaisePlayerSelected(1);
                    }
                }
                break;
            case GameState.Playing:

                if (isCountdownAllowed)
                {
                    startCountdown -= Time.deltaTime;
                    if (startCountdown < 0)
                    {
                        BeginMatch();
                        isCountdownAllowed = false;
                    }
                }
                else
                {
                    _matchTimer -= Time.deltaTime;
                    if (_matchTimer <= 0f)
                    {
                        _matchTimer = 0f;
                        EndMatch();
                    }
                }

                UpdateUI();
                break;
            case GameState.Paused:
                break;
            case GameState.Finished:
                break;
            default:
                break;
        }
    }

    // ---------- Observer outputs ----------
    private void SetPlayerUI(int playerId, bool visible)
    {
        RaiseTogglePlayerUI(playerId, visible);
    }

    private void BeginMatch()
    {
        _running = true;
        _matchTimer = matchLengthSeconds;

        SetPlayerUI(0, true);
        SetPlayerUI(1, true);

        RaiseMatchStarted();
    }

    private void EndMatch()
    {
        if (!_running) return;
        _running = false;

        SetPlayerUI(0, false);
        SetPlayerUI(1, false);

        RaiseMatchEnded();
        RaiseGameState(GameState.Finished);
    }

    // ---------- Observer inputs ----------
    private void OnCalibrationDone(int playerId)
    {
        _calibDone.Add(playerId);
        if (_calibDone.Count >= 2)
        {
            StartCoroutine(CountdownCoroutine(changeToPlayingStateDuration));
        }
    }

    // ---------- Internal ----------

    private System.Collections.IEnumerator CountdownCoroutine(float seconds)
    {
        float t = seconds;
        while (t > 0f)
        {
            RaiseCountdown(Mathf.Ceil(t));
            t -= Time.deltaTime;
            yield return null;
        }

        RaiseGameState(GameState.Playing);
        isCountdownAllowed = true;
    }

    private void UpdateUI()
    {
        if (isCountdownAllowed)
        {
            if (startCountdown > 1)
            {
                StartCountdownText.text = startCountdown.ToString("0");
            }
            else
            {
                StartCountdownText.text = "GO";
            }
        }

        StartCountdownText.gameObject.SetActive(isCountdownAllowed);
        GameCountdownText.text = _matchTimer.ToString("00");
        GameCountdownText.transform.parent.gameObject.SetActive(!isCountdownAllowed);
    }

    public bool IsPlatformPC()
    {
#if PLATFORM_ANDROID
        return false;
#else
        return true;
#endif
    }
}
