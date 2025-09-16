using UnityEngine;

// Move this game object to main menu
[DefaultExecutionOrder(-100)]
public class GameBootstrapper : MonoBehaviour
{
    // Replace later with main menu selection
    [SerializeField] private GameMode gameMode;

    void Awake()
    {
        var cfg = (GameContext.Instance != null)
            ? GameContext.Instance.Current
            : new GameConfig { mode = gameMode };

        if (GameContext.Instance == null)
        {
            var go = new GameObject("GameContext");
            var ctx = go.AddComponent<GameContext>();
            ctx.SetConfig(cfg);
        }
    }
}
