using UnityEngine.SceneManagement;

public static class SceneTransit
{
    public static string NextScene { get; private set; }
    public static LoadingProfile Profile { get; private set; }

    public static void Go(string targetScene, LoadingProfile profile = null)
    {
        NextScene = targetScene;
        Profile = profile;

        var active = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (active == "Transition") return;

        SceneManager.LoadScene("Transition", LoadSceneMode.Single);
    }

    public static void Clear()
    {
        NextScene = null;
        Profile = null;
    }
}
