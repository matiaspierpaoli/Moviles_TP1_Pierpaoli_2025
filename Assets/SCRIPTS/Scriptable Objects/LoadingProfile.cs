using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Loading/Loading Profile")]
public class LoadingProfile : ScriptableObject
{
    [Header("Visual")]
    public bool showProgressBar = true;
    public bool shouldShowHint = false;
    public float minDisplayTime = 2.0f;
    public Sprite backgoundSprite;
    public AnimationCurve fakeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Progreso ponderado")]
    [Range(0, 1)] public float bootStepsWeight = 0.35f;
}
