using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreditsScroll : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform viewport;

    [Header("Motion")]
    [SerializeField] float speed = 50f;
    [SerializeField] float startDelay = 0.5f;
    [SerializeField] float endDelay = 1f;
    [SerializeField] bool unscaledTime = true;

    [Header("Events")]
    public UnityEvent onFinished;

    float targetY;
    bool running;

    IEnumerator Start()
    {
        if (!content) content = GetComponent<RectTransform>();
        if (!viewport) viewport = content.parent as RectTransform;

        float vh = viewport.rect.height;
        float ch = content.rect.height;

        targetY = ch + vh;

        yield return new WaitForSecondsRealtime(startDelay);
        running = true;
    }

    void Update()
    {
        if (!running) return;

        float dt = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        content.anchoredPosition += Vector2.up * speed * dt;

        if (content.anchoredPosition.y >= 0)
        {
            running = false;
            if (endDelay > 0f)
                StartCoroutine(FireFinishedAfter(endDelay));
            else
                onFinished?.Invoke();
        }
    }

    System.Collections.IEnumerator FireFinishedAfter(float t)
    {
        if (unscaledTime) yield return new WaitForSecondsRealtime(t);
        else yield return new WaitForSeconds(t);
        onFinished?.Invoke();
    }
}
