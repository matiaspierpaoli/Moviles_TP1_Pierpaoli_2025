using UnityEngine;

[CreateAssetMenu(fileName = "DifficultySettings", menuName = "Game/Difficulty Settings")]
public class DifficultySettings : ScriptableObject
{
    [Header("Spawn Rates")]
    public float bagSpawnInterval = 2f;
    public float obstacleSpawnInterval = 5f;
    public float taxiSpawnInterval = 10f;

    [Header("Maximum Active Objects")]
    public int maxBags = 15;
    public int maxCones = 8;
    public int maxBoxes = 6;
    public int maxTaxis = 4;
    public int maxDeposits = 4;

    [Header("Initial Spawn Counts")]
    public int initialBags = 5;
    public int initialCones = 3;
    public int initialBoxes = 2;
    public int initialTaxis = 1;
    public int initialDeposits = 1;

    [Header("Difficulty Modifiers")]
    [Range(0f, 1f)] public float bagSpawnChance = 0.7f;
    [Range(0f, 1f)] public float obstacleSpawnChance = 0.5f;
    public float taxiSpeedMultiplier = 1f;
}