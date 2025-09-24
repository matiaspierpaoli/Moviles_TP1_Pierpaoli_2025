using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransit
{
    public static string NextScene { get; private set; }

    public static void Go(string targetScene)
    {
        NextScene = targetScene;
        SceneManager.LoadScene("Transition", LoadSceneMode.Single);
    }
}
