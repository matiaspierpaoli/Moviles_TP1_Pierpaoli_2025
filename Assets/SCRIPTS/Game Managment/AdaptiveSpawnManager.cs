using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using static GameSignals;

public class AdaptiveSpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public SpawnPoints spawnPoints;

    [Header("Factory Reference")]
    public GameObjectFactory factory;

    [Header("Difficulty Settings")]
    public DifficultySettings easySettings;
    public DifficultySettings mediumSettings;
    public DifficultySettings hardSettings;

    private DifficultySettings currentDifficulty;
    private Dictionary<System.Type, List<GameObject>> activeObjects = new Dictionary<System.Type, List<GameObject>>();

    private int currentBags = 0;
    private int currentCones = 0;
    private int currentBoxes = 0;
    private int currentTaxis = 0;

    private Coroutine bagSpawnCoroutine;
    private Coroutine obstacleSpawnCoroutine;
    private Coroutine taxiSpawnCoroutine;

    private bool spawningEnabled = false;

    private float cleanupTimer = 0f;
    private const float CLEANUP_INTERVAL = 1f;

    private void OnEnable()
    {
        GameDiffucltyChanged += SetDifficulty;
        MatchCountdownStarted += OnMatchCountdownStarted;
        MatchEnded += OnMatchEnded;
    }

    private void OnDisable()
    {
        GameDiffucltyChanged -= SetDifficulty;
        MatchCountdownStarted -= OnMatchCountdownStarted;
        MatchEnded -= OnMatchEnded;
    }

    public void SetDifficulty(GameDifficulty difficulty)
    {
        currentDifficulty = difficulty switch
        {
            GameDifficulty.Medium => mediumSettings,
            GameDifficulty.Hard => hardSettings,
            _ => easySettings
        };

        factory.SetDifficulty(currentDifficulty);

        StopAllSpawning();

        Debug.Log($"Factory difficulty set to: {difficulty}");
    }

    private void OnMatchCountdownStarted()
    {
        spawningEnabled = true;
        StartAllSpawning();
        SpawnInitialObjects();
    }

    private void OnMatchEnded()
    {
        spawningEnabled = false;
        StopAllSpawning();
        ClearAllObjects();
    }

    private void StartAllSpawning()
    {
        if (!spawningEnabled) return;

        bagSpawnCoroutine = StartCoroutine(BagSpawningRoutine());
        obstacleSpawnCoroutine = StartCoroutine(ObstacleSpawningRoutine());
        taxiSpawnCoroutine = StartCoroutine(TaxiSpawningRoutine());
    }

    private void StopAllSpawning()
    {
        if (bagSpawnCoroutine != null) StopCoroutine(bagSpawnCoroutine);
        if (obstacleSpawnCoroutine != null) StopCoroutine(obstacleSpawnCoroutine);
        if (taxiSpawnCoroutine != null) StopCoroutine(taxiSpawnCoroutine);
    }

    private void SpawnInitialObjects()
    {
        if (currentDifficulty == null) return;

        Debug.Log($"Spawning initial objects for difficulty: {currentDifficulty.name}");

        for (int i = 0; i < currentDifficulty.initialBags; i++)
        {
            if (currentBags < currentDifficulty.maxBags && spawnPoints.bagSpawns.Count > 0)
            {
                SpawnRandomBag();
            }
        }

        for (int i = 0; i < currentDifficulty.initialCones; i++)
        {
            if (currentCones < currentDifficulty.maxCones && spawnPoints.coneSpawns.Count > 0)
            {
                SpawnSpecificObstacle(ObstacleType.Cone);
            }
        }

        for (int i = 0; i < currentDifficulty.initialBoxes; i++)
        {
            if (currentBoxes < currentDifficulty.maxBoxes && spawnPoints.boxSpawns.Count > 0)
            {
                SpawnSpecificObstacle(ObstacleType.Box);
            }
        }

        for (int i = 0; i < currentDifficulty.initialTaxis; i++)
        {
            if (currentTaxis < currentDifficulty.maxTaxis && spawnPoints.taxiSpawns.Count > 0)
            {
                SpawnRandomTaxi();
            }
        }

        SpawnInitialDeposits();

        Debug.Log($"Initial spawn complete - Bags: {currentBags}, Cones: {currentCones}, Boxes: {currentBoxes}, Taxis: {currentTaxis}");
    }

    private void SpawnInitialDeposits()
    {
        if (spawnPoints.depositSpawns.Count == 0) return;

        List<Transform> availableSpawns = new List<Transform>(spawnPoints.depositSpawns);

        int depositsToSpawn = Mathf.Min(currentDifficulty.initialDeposits, availableSpawns.Count);

        for (int i = 0; i < depositsToSpawn; i++)
        {
            if (availableSpawns.Count == 0) break;

            int randomIndex = Random.Range(0, availableSpawns.Count);
            Transform spawnPoint = availableSpawns[randomIndex];
            availableSpawns.RemoveAt(randomIndex);

            SpawnDepositAt(spawnPoint);
        }
    }

    private void SpawnDepositAt(Transform spawnPoint)
    {
        GameObject deposit = factory.CreateDeposit(spawnPoint.position, spawnPoint.rotation);
        RegisterObject(deposit, typeof(Deposito2));
    }

    private void SpawnSpecificObstacle(ObstacleType type)
    {
        List<Transform> spawnList = type == ObstacleType.Cone ? spawnPoints.coneSpawns : spawnPoints.boxSpawns;

        if (spawnList.Count == 0) return;

        Transform spawnPoint = GetRandomSpawnPoint(spawnList);
        GameObject obstacle = factory.CreateObstacle(type, spawnPoint.position, spawnPoint.rotation);

        if (type == ObstacleType.Cone)
        {
            RegisterObject(obstacle, typeof(Obstaculo));
            currentCones++;
        }
        else
        {
            RegisterObject(obstacle, typeof(Obstaculo));
            currentBoxes++;
        }
    }

    private void ClearAllObjects()
    {
        foreach (var kvp in activeObjects)
        {
            foreach (GameObject obj in kvp.Value)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            kvp.Value.Clear();
        }

        currentBags = 0;
        currentCones = 0;
        currentBoxes = 0;
        currentTaxis = 0;

        Debug.Log("All objects cleared");
    }

    private IEnumerator BagSpawningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentDifficulty.bagSpawnInterval);

            if (currentBags < currentDifficulty.maxBags &&
                spawnPoints.bagSpawns.Count > 0 &&
                Random.value <= currentDifficulty.bagSpawnChance)
            {
                SpawnRandomBag();
            }
        }
    }

    private IEnumerator ObstacleSpawningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentDifficulty.obstacleSpawnInterval);

            if (spawnPoints.coneSpawns.Count > 0 &&
                spawnPoints.boxSpawns.Count > 0 &&
                Random.value <= currentDifficulty.obstacleSpawnChance)
            {
                SpawnRandomObstacle();
            }
        }
    }

    private IEnumerator TaxiSpawningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentDifficulty.taxiSpawnInterval);

            if (currentTaxis < currentDifficulty.maxTaxis &&
                spawnPoints.taxiSpawns.Count > 0)
            {
                SpawnRandomTaxi();
            }
        }
    }

    private void SpawnRandomBag()
    {
        if (spawnPoints.bagSpawns.Count == 0) return;

        Transform spawnPoint = GetRandomSpawnPoint(spawnPoints.bagSpawns);
        GameObject bag = factory.CreateBag(spawnPoint.position, spawnPoint.rotation);

        RegisterObject(bag, typeof(Bolsa));
        currentBags++;

        Bolsa bolsaComponent = bag.GetComponent<Bolsa>();
        if (bolsaComponent != null)
        {
            StartCoroutine(MonitorBagCollection(bolsaComponent));
        }
    }

    private void SpawnRandomObstacle()
    {
        bool canSpawnCone = currentCones < currentDifficulty.maxCones;
        bool canSpawnBox = currentBoxes < currentDifficulty.maxBoxes;

        if (!canSpawnCone && !canSpawnBox) return;

        ObstacleType typeToSpawn;
        List<Transform> spawnList;

        if (canSpawnCone && canSpawnBox)
        {
            typeToSpawn = Random.Range(0, 2) == 0 ? ObstacleType.Cone : ObstacleType.Box;
            spawnList = typeToSpawn == ObstacleType.Cone ? spawnPoints.coneSpawns : spawnPoints.boxSpawns;
        }
        else if (canSpawnCone)
        {
            typeToSpawn = ObstacleType.Cone;
            spawnList = spawnPoints.coneSpawns;
        }
        else
        {
            typeToSpawn = ObstacleType.Box;
            spawnList = spawnPoints.boxSpawns;
        }

        if (spawnList.Count == 0) return;

        Transform spawnPoint = GetRandomSpawnPoint(spawnList);
        GameObject obstacle = factory.CreateObstacle(typeToSpawn, spawnPoint.position, spawnPoint.rotation);

        if (typeToSpawn == ObstacleType.Cone)
        {
            RegisterObject(obstacle, typeof(Obstaculo));
            currentCones++;
        }
        else
        {
            RegisterObject(obstacle, typeof(Obstaculo));
            currentBoxes++;
        }
    }

    private void SpawnRandomTaxi()
    {
        if (spawnPoints.taxiSpawns.Count == 0) return;

        Transform spawnPoint = GetRandomSpawnPoint(spawnPoints.taxiSpawns);
        GameObject taxi = factory.CreateTaxi(spawnPoint.position, spawnPoint.rotation);

        RegisterObject(taxi, typeof(TaxiComp));
        currentTaxis++;
    }

    private Transform GetRandomSpawnPoint(List<Transform> spawnList)
    {
        return spawnList[Random.Range(0, spawnList.Count)];
    }

    private void RegisterObject(GameObject obj, System.Type type)
    {
        if (!activeObjects.ContainsKey(type))
        {
            activeObjects[type] = new List<GameObject>();
        }
        activeObjects[type].Add(obj);
    }

    private IEnumerator MonitorBagCollection(Bolsa bolsa)
    {
        yield return new WaitUntil(() => bolsa.Desapareciendo);

        yield return new WaitForSeconds(bolsa.TiempParts);

        currentBags--;
        UnregisterObject(bolsa.gameObject, typeof(Bolsa));
    }

    private void UnregisterObject(GameObject obj, System.Type type)
    {
        if (activeObjects.ContainsKey(type) && activeObjects[type].Contains(obj))
        {
            activeObjects[type].Remove(obj);
        }
    }

    private void CleanupDestroyedObjects()
    {
        foreach (var kvp in activeObjects)
        {
            kvp.Value.RemoveAll(obj => obj == null);
        }

        RecalculateCounters();
    }

    private void RecalculateCounters()
    {
        currentBags = GetActiveCount(typeof(Bolsa));
        currentCones = GetActiveCount(typeof(Obstaculo));
        currentBoxes = 0;
        currentTaxis = GetActiveCount(typeof(TaxiComp));
    }

    private int GetActiveCount(System.Type type)
    {
        return activeObjects.ContainsKey(type) ? activeObjects[type].Count : 0;
    }

    private void Update()
    {
        cleanupTimer += Time.deltaTime;
        if (cleanupTimer >= CLEANUP_INTERVAL)
        {
            cleanupTimer = 0f;
            CleanupDestroyedObjects();
        }
    }

    private void OnDestroy()
    {
        StopAllSpawning();
    }
}