using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnPoints
{
    public List<Transform> bagSpawns = new List<Transform>();
    public List<Transform> coneSpawns = new List<Transform>();
    public List<Transform> boxSpawns = new List<Transform>();
    public List<Transform> taxiSpawns = new List<Transform>();
    public List<Transform> depositSpawns = new List<Transform>();
}