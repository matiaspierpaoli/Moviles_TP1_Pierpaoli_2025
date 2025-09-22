using UnityEngine;

public enum ObstacleType
{
    Cone,
    Box
}

public class GameObjectFactory : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bagPrefab;
    public GameObject taxiPrefab;
    public GameObject conePrefab;
    public GameObject boxPrefab;
    public GameObject depositPrefab;

    private DifficultySettings currentDifficulty;

    public void SetDifficulty(DifficultySettings difficulty)
    {
        currentDifficulty = difficulty;
    }

    public GameObject CreateBag(Vector3 position, Quaternion rotation)
    {
        GameObject bag = Instantiate(bagPrefab, position, rotation);
        Bolsa bolsaComponent = bag.GetComponent<Bolsa>();

        if (bolsaComponent != null && currentDifficulty != null)
        {
            int randomValue = Random.Range(
                currentDifficulty.bagBaseValue - currentDifficulty.bagValueRandomness,
                currentDifficulty.bagBaseValue + currentDifficulty.bagValueRandomness
            );

            randomValue = Mathf.Max(10000, randomValue);

            bolsaComponent.Monto = (Pallet.Valores)randomValue;
        }

        return bag;
    }

    public GameObject CreateTaxi(Vector3 position, Quaternion rotation)
    {
        GameObject taxi = Instantiate(taxiPrefab, position, rotation);
        TaxiComp taxiComp = taxi.GetComponent<TaxiComp>();

        if (taxiComp != null && currentDifficulty != null)
        {
            taxiComp.Vel *= currentDifficulty.taxiSpeedMultiplier;
        }

        return taxi;
    }

    public GameObject CreateDeposit(Vector3 position, Quaternion rotation)
    {
        GameObject deposit = Instantiate(depositPrefab, position, rotation);

        return deposit;
    }

    public GameObject CreateObstacle(ObstacleType type, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = type == ObstacleType.Cone ? conePrefab : boxPrefab;
        GameObject obstacle = Instantiate(prefab, position, rotation);

        return obstacle;
    }
}