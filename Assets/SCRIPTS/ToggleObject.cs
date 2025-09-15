using System.Collections;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;
    [SerializeField] private float toggleInterval = 1f;

    private Coroutine toggleCoroutine;

    private void Start()
    {
        if (objectToToggle != null)
            toggleCoroutine = StartCoroutine(ToggleLoop());
    }

    private IEnumerator ToggleLoop()
    {
        var wait = new WaitForSeconds(toggleInterval);

        while (enabled && objectToToggle != null)
        {
            objectToToggle.SetActive(!objectToToggle.activeSelf);
            yield return wait;
        }
    }

    private void OnDisable()
    {
        if (toggleCoroutine != null)
            StopCoroutine(toggleCoroutine);
    }
}
