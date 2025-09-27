using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private bool shouldLoadProfile = false;
    [SerializeField] private LoadingProfile loadProfile;

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void LoadLevelWithTransition(string sceneName)
    {
        if (shouldLoadProfile)
            SceneTransit.Go(sceneName, loadProfile);
        else
            SceneTransit.Go(sceneName);
    }

    public void LoadLevelAsync(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
