using UnityEngine;

public class DifficultySelector : MonoBehaviour
{
    public void SetEasyDifficulty()
    {
        if (GameContext.Instance)
            GameContext.Instance.SetDifficulty(GameDifficulty.Easy);
    }

    public void SetMediumDifficulty()
    {
        if (GameContext.Instance)
            GameContext.Instance.SetDifficulty(GameDifficulty.Medium);
    }

    public void SetHardDifficulty()
    {
        if (GameContext.Instance)
            GameContext.Instance.SetDifficulty(GameDifficulty.Hard);
    }
}
