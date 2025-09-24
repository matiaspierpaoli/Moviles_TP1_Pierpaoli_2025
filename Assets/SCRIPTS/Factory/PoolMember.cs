using UnityEngine;
using System;

public sealed class PoolMember : MonoBehaviour
{
    public Action<PoolMember> ReturnToPool;

    void OnDisable()
    {
        ReturnToPool?.Invoke(this);
    }
}
