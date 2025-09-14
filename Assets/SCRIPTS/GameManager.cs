using System.Collections.Generic;
using UnityEngine;

using static GameSignals;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Match")]
    [SerializeField] private float matchLengthSeconds = 60f;
    [SerializeField] private bool autoStartAfterBothTutorials = true;

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
    }

    private void OnDisable()
    {
        CalibrationDone -= OnCalibrationDone;
    }

    private void Start()
    {
        _running = false;
        _matchTimer = matchLengthSeconds;

        RaisePlayerSideAssigned(0, PlayerSide.Left);
        RaisePlayerSideAssigned(1, PlayerSide.Right);

        RaiseCalibrationStarted();
        RaiseGameState(GameState.Calibrating);
    }

    private void Update()
    {
        if (!_running) return;

        _matchTimer -= Time.deltaTime;
        if (_matchTimer <= 0f)
        {
            _matchTimer = 0f;
            EndMatch();
        }
    }

    // ---------- Observer outputs ----------
    private void SetPlayerUI(int playerId, bool visible)
    {
        RaiseTogglePlayerUI(playerId, visible);
    }

    private void StartCountdownThenMatch(float seconds = 3f)
    {
        StartCoroutine(CountdownCoroutine(seconds));
    }

    private System.Collections.IEnumerator CountdownCoroutine(float seconds)
    {
        RaiseGameState(GameState.Playing);
        float t = seconds;
        while (t > 0f)
        {
            RaiseCountdown(Mathf.Ceil(t));
            t -= Time.deltaTime;
            yield return null;
        }
        BeginMatch();
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
            StartCountdownThenMatch(3f);
        }
    }
}
