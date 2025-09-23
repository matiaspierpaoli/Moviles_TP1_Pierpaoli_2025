using UnityEngine;

// Move this game object to main menu
[DefaultExecutionOrder(-100)]
public class GameBootstrapper : MonoBehaviour
{
    // Replace later with main menu selection
    [SerializeField] private GameMode gameMode;
    [SerializeField] private GameDifficulty gameDifficulty;

    void Awake()
    {
        var cfg = (GameContext.Instance != null)
            ? GameContext.Instance.Current
            : new GameConfig { mode = gameMode, difficulty = gameDifficulty, player1Money = 0, player2Money = 0 };

        if (GameContext.Instance == null)
        {
            var go = new GameObject("GameContext");
            var ctx = go.AddComponent<GameContext>();
            ctx.SetConfig(cfg);
        }
    }

    public void SetSingleplayerMode()
    {
        if (GameContext.Instance)
            GameContext.Instance.Current.mode = GameMode.SinglePlayer;
    }

    public void SetMultiplayerMode()
    {
        if (GameContext.Instance)
            GameContext.Instance.Current.mode = GameMode.Multiplayer;
    }
}
