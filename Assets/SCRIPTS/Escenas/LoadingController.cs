using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider progressBar;

    [Header("Timing")]
    [SerializeField] private float minDisplayTime = 2.2f;
    [SerializeField]
    private AnimationCurve fakeCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Start() => StartCoroutine(LoadNext());

    IEnumerator LoadNext()
    {
        string target = SceneTransit.NextScene;
        var op = SceneManager.LoadSceneAsync(target, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        float elapsed = 0f;
        float visual = 0f;

        while (true)
        {
            elapsed += Time.unscaledDeltaTime;

            float real = Mathf.Clamp01(op.progress / 0.9f);

            float t = Mathf.Clamp01(elapsed / minDisplayTime);
            float fake = fakeCurve.Evaluate(t);

            float targetVisual = Mathf.Min(fake, real);

            visual = Mathf.MoveTowards(visual, targetVisual, 2.5f * Time.unscaledDeltaTime);
            if (progressBar) progressBar.value = visual;

            if (t >= 1f && real >= 1f) break;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.25f);

        op.allowSceneActivation = true;
    }
}
