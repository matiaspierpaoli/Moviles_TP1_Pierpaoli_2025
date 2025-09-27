using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Slider progressBar;
    [SerializeField] string defaultLevelToLoad = "MainMenu";
    [SerializeField] bool shouldShowHint = false;
    [SerializeField] Image backgoundImage;
    [SerializeField] Sprite defaultBackgoundSprite;
    [SerializeField] LoadingProfile defaultProfile;
    [SerializeField] GameObject hintParent;

    float minDisplayTime;
    AnimationCurve fakeCurve;

    void Awake()
    {
        var p = SceneTransit.Profile ? SceneTransit.Profile : defaultProfile;

        minDisplayTime = p ? p.minDisplayTime : 2.0f;
        fakeCurve = p ? p.fakeCurve : AnimationCurve.EaseInOut(0, 0, 1, 1);

        if (backgoundImage)
            backgoundImage.sprite = p && p.backgoundSprite ? p.backgoundSprite : defaultBackgoundSprite;

        shouldShowHint = p ? p.shouldShowHint : shouldShowHint;
        if (hintParent) hintParent.SetActive(shouldShowHint);
        if (progressBar && p) progressBar.gameObject.SetActive(p.showProgressBar);
    }

    void Start() => StartCoroutine(Run());

    IEnumerator Run()
    {
        string target = !string.IsNullOrEmpty(SceneTransit.NextScene)
                        ? SceneTransit.NextScene
                        : defaultLevelToLoad;

        var op = SceneManager.LoadSceneAsync(target, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        float visual = 0f, elapsed = 0f;

        while (true)
        {
            elapsed += Time.unscaledDeltaTime;

            float real = Mathf.Clamp01(op.progress / 0.9f);

            float t = Mathf.Clamp01(elapsed / minDisplayTime);
            float fake = fakeCurve.Evaluate(t);

            float targetVisual = Mathf.Min(real, fake);

            visual = Mathf.MoveTowards(visual, targetVisual, 2.5f * Time.unscaledDeltaTime);
            if (progressBar) progressBar.value = visual;

            if (t >= 1f && real >= 1f) break;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.2f);
        op.allowSceneActivation = true;

        SceneTransit.Clear();
    }
}
