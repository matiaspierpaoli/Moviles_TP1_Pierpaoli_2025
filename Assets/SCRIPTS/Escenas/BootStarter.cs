using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class BootStarter : MonoBehaviour
{
    [SerializeField] LoadingProfile bootProfile;  // LP_Boot

    void Awake()
    {
        if (!string.IsNullOrEmpty(SceneTransit.NextScene))
            return;

        SceneTransit.Go(
            targetScene: "MainMenu",
            profile: bootProfile
        );
    }
}
