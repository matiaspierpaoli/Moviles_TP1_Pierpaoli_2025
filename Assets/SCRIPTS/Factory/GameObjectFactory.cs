using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum ObstacleType
{
    Cone,
    Box
}

public class GameObjectFactory : MonoBehaviour
{
    [Header("Addressable Refs")]
    public AssetReferenceGameObject bagRef;
    public AssetReferenceGameObject taxiRef;
    public AssetReferenceGameObject coneRef;
    public AssetReferenceGameObject boxRef;
    public AssetReferenceGameObject depositRef;

    private DifficultySettings currentDifficulty;

    readonly Dictionary<string, Queue<PoolMember>> pools = new();
    readonly Dictionary<GameObject, string> keyByInstance = new();
    readonly List<AsyncOperationHandle<GameObject>> warmupHandles = new();

    public void SetDifficulty(DifficultySettings difficulty)
    {
        currentDifficulty = difficulty;
    }

    public async Task WarmupAsync(DifficultySettings difficulty)
    {
        SetDifficulty(difficulty);

        await PreloadAsync("bag", bagRef, difficulty.initialBags + 4);
        await PreloadAsync("cone", coneRef, difficulty.initialCones + 2);
        await PreloadAsync("box", boxRef, difficulty.initialBoxes + 2);
        await PreloadAsync("taxi", taxiRef, difficulty.initialTaxis + 2);
        await PreloadAsync("deposit", depositRef, difficulty.initialDeposits + 1);
    }

    async Task PreloadAsync(string key, AssetReferenceGameObject aref, int count)
    {
        if (!pools.ContainsKey(key)) pools[key] = new Queue<PoolMember>();

        for (int i = 0; i < count; i++)
        {
            var h = aref.InstantiateAsync(Vector3.zero, Quaternion.identity, transform);
            await h.Task;
            var go = h.Result;
            warmupHandles.Add(h);

            var pm = go.GetComponent<PoolMember>() ?? go.AddComponent<PoolMember>();
            pm.ReturnToPool = (m) => ReturnInternal(key, m);

            go.SetActive(false);
            pools[key].Enqueue(pm);
            keyByInstance[go] = key;
        }
    }

    public void ReleaseAll()
    {
        foreach (var h in warmupHandles)
            if (h.IsValid()) Addressables.ReleaseInstance(h);
        warmupHandles.Clear();

        pools.Clear();
        keyByInstance.Clear();
    }

    void ReturnInternal(string key, PoolMember m)
    {
        m.gameObject.SetActive(false);
        pools[key].Enqueue(m);
    }

    public GameObject CreateBag(Vector3 pos, Quaternion rot)
    {
        var go = SpawnFromPool("bag", bagRef, pos, rot);
        var bolsa = go.GetComponent<Bolsa>();

        return go;
    }

    public GameObject CreateTaxi(Vector3 pos, Quaternion rot)
    {
        var go = SpawnFromPool("taxi", taxiRef, pos, rot);
        var t = go.GetComponent<TaxiComp>();
        if (t && currentDifficulty != null) t.Vel *= currentDifficulty.taxiSpeedMultiplier;
        return go;
    }

    public GameObject CreateObstacle(ObstacleType type, Vector3 pos, Quaternion rot)
    {
        string key = (type == ObstacleType.Cone) ? "cone" : "box";
        var aref = (type == ObstacleType.Cone) ? coneRef : boxRef;
        return SpawnFromPool(key, aref, pos, rot);
    }

    public GameObject CreateDeposit(Vector3 pos, Quaternion rot)
    {
        return SpawnFromPool("deposit", depositRef, pos, rot);
    }

    GameObject SpawnFromPool(string key, AssetReferenceGameObject aref, Vector3 pos, Quaternion rot)
    {
        PoolMember pm;
        if (!pools.TryGetValue(key, out var q) || q.Count == 0)
        {
            var h = aref.InstantiateAsync(pos, rot, transform);
            h.WaitForCompletion();
            var go = h.Result;
            warmupHandles.Add(h);
            pm = go.GetComponent<PoolMember>() ?? go.AddComponent<PoolMember>();
            pm.ReturnToPool = (m) => ReturnInternal(key, m);
            keyByInstance[go] = key;
            return go;
        }

        pm = q.Dequeue();
        pm.transform.SetPositionAndRotation(pos, rot);
        pm.gameObject.SetActive(true);
        return pm.gameObject;
    }

    public void Despawn(GameObject go)
    {
        if (!go) return;
        if (keyByInstance.TryGetValue(go, out var key))
        {
            var pm = go.GetComponent<PoolMember>();
            ReturnInternal(key, pm);
        }
        else
        {
            Addressables.ReleaseInstance(go);
        }
    }
}