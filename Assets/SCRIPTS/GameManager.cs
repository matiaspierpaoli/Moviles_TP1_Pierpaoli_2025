using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameSignals;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }
    public GameConfig Config { get; private set; }
    private GameState currentGameState = GameState.Calibrating;

    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string ptsFinalSceneName = "PtsFinal";

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

    public Player Player1 => player1;
    public Player Player2 => player2;

    private readonly HashSet<int> _calibDone = new();
    private float _matchTimer;
    private bool _running;

    private Action<GameState> gameStateChangedHandler;

    private void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
        DontDestroyOnLoad(gameObject);

        player1.transform.gameObject.SetActive(false);
        player2.transform.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        CalibrationDone += OnCalibrationDone;
        gameStateChangedHandler += gameState => currentGameState = gameState;
        GameStateChanged += gameStateChangedHandler;
        PlayerFinished += OnPlayerFinished;
    }

    private void OnDisable() 
    { 
        CalibrationDone -= OnCalibrationDone; 
        GameStateChanged -= gameStateChangedHandler; 
        PlayerFinished -= OnPlayerFinished;
    }
    
    private void Start() 
    { 
        Config = GameContext.Instance?.Current ?? new GameConfig { mode = GameMode.Multiplayer, player1Money = 0, player2Money = 0 }; 
        Debug.Log("Game mode: " + Config.mode.ToString()); 
        
        SetupPlayers(Config.mode); 
        
        _running = false; _matchTimer = matchLengthSeconds; 
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
                    if (InputManager.Instance.IsUpPressed(verticalInputName, Player1.IdPlayer.ToString()))
                    {
                        RaisePlayerSelected(Player1.IdPlayer);
                    }
                }

                if (!player2.selected)
                {
                    if (InputManager.Instance.IsUpPressed(verticalInputName, Player2.IdPlayer.ToString()))
                    {
                        RaisePlayerSelected(Player2.IdPlayer);
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
                EndMatch();
                sceneLoader.LoadLevel(ptsFinalSceneName);

                break;
            default:
                break;
        }
    }

    // ---------- Observer outputs ----------



    // ---------- Observer inputs ----------
    private void OnCalibrationDone(int playerId)
    {
        _calibDone.Add(playerId);
        if (_calibDone.Count >= 2 && Config.mode == GameMode.Multiplayer)
        {
            StartCoroutine(CountdownCoroutine(changeToPlayingStateDuration));
        }
        else if (Config.mode == GameMode.SinglePlayer)
        {
            StartCoroutine(CountdownCoroutine(changeToPlayingStateDuration));
        }
    }

    private void OnPlayerFinished(int playerId)
    {
        if (playerId == Player1.IdPlayer)
            Player1.finishedRace = true;
        else if (playerId == Player2.IdPlayer)
            Player2.finishedRace = true;

        if ((Config.mode == GameMode.SinglePlayer && playerId == player1.IdPlayer) ||
            (Config.mode == GameMode.Multiplayer && player1.finishedRace && player2.finishedRace))
        {
            RaiseGameState(GameState.Finished);
        }
    }

    // ---------- Internal ----------

    private void BeginMatch()
    {
        _running = true;
        _matchTimer = matchLengthSeconds;

        RaiseMatchStarted();
    }

    private void EndMatch()
    {
        if (!_running) return;
        _running = false;

        RaiseMatchEnded();
        RaiseGameState(GameState.Finished);
    }

    private void SetupPlayers(GameMode mode) 
    { 
        player1.gameObject.SetActive(true);

        switch (mode) 
        { 
            case GameMode.SinglePlayer: 
                RaiseSingleplayerActive(); 
                RaisePlayerSideAssigned(Player1.IdPlayer, PlayerSide.Middle); 
                break; 
            case GameMode.Multiplayer: 
                player2.gameObject.SetActive(true); 
                RaiseMultiplayerActive(); 
                RaisePlayerSideAssigned(Player1.IdPlayer, PlayerSide.Left); 
                RaisePlayerSideAssigned(Player2.IdPlayer, PlayerSide.Right); 
                break; 
        } 
    }

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
