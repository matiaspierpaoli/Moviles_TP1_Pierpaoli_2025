using UnityEngine;
using UnityEngine.SceneManagement;
using System;

// Agressive execution orden to guarantee correct instancing if object is in scene
[DefaultExecutionOrder(-10000)]
public sealed class GameContext : MonoBehaviour
{
    public static GameContext Instance { get; private set; }

    [SerializeField] private GameConfig current = new GameConfig(); // instancia viva
    public GameConfig Current => current;

    public event Action<GameConfig> ConfigChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { Instance = null; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetConfig(GameConfig cfg)
    {
        current.mode = cfg.mode;
        current.playerCount = cfg.playerCount;
        current.player1Money = cfg.player1Money;
        current.player2Money = cfg.player2Money;

        ConfigChanged?.Invoke(current);
    }

    public void SetPlayerMoney(int playerId, int money)
    {
        if (playerId == 0) current.player1Money = money;
        else if (playerId == 1) current.player2Money = money;

        ConfigChanged?.Invoke(current);
    }

    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
