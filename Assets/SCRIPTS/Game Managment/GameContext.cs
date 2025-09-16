using UnityEngine;
using UnityEngine.SceneManagement;
using System;

// Agressive execution orden to guarantee correct instancing if object is in scene
[DefaultExecutionOrder(-10000)]
public sealed class GameContext : MonoBehaviour
{
    public static GameContext Instance { get; private set; }

    public GameConfig Current { get; private set; }
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
        Current = cfg;
        ConfigChanged?.Invoke(Current);
    }

    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
