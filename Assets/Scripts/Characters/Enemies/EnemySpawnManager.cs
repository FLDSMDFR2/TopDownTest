using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField]
    protected List<GameObject> SpawnPrefabsEasy;

    [SerializeField]
    protected List<GameObject> SpawnPrefabsMedium;

    [SerializeField]
    protected List<GameObject> SpawnPrefabsHard;

    [SerializeField]
    protected List<GameObject> SpawnPrefabsInsane;



    [SerializeField]
    protected static List<GameObject> spawnPrefabsEasy;

    [SerializeField]
    protected static List<GameObject> spawnPrefabsMedium;

    [SerializeField]
    protected static List<GameObject> spawnPrefabsHard;

    [SerializeField]
    protected static List<GameObject> spawnPrefabsInsane;

    private void Awake()
    {
        spawnPrefabsEasy = SpawnPrefabsEasy;
        spawnPrefabsMedium = SpawnPrefabsMedium;
        spawnPrefabsHard = SpawnPrefabsHard;
        spawnPrefabsInsane = SpawnPrefabsInsane;
    }

    public static GameObject GetEnemySpawnByDifficulty(RoomDifficulty difficulty)
    {
        switch (difficulty)
        {
            case RoomDifficulty.Easy:
                return spawnPrefabsEasy[RandomGenerator.SeededRange(0, spawnPrefabsEasy.Count-1)];
            case RoomDifficulty.Medium:
                return spawnPrefabsMedium[RandomGenerator.SeededRange(0, spawnPrefabsMedium.Count - 1)];
            case RoomDifficulty.Hard:
                return spawnPrefabsHard[RandomGenerator.SeededRange(0, spawnPrefabsHard.Count - 1)];
            case RoomDifficulty.Insane:
                return spawnPrefabsInsane[RandomGenerator.SeededRange(0, spawnPrefabsInsane.Count - 1)];
            default:
                TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "difficulty Type not found " + difficulty.ToString());
                return spawnPrefabsEasy[0];
        }
    }
 }
