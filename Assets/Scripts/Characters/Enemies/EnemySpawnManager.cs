using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField]
    protected List<GameObject> SpwanPrefabsEasy;

    [SerializeField]
    protected List<GameObject> SpwanPrefabsMedium;

    [SerializeField]
    protected List<GameObject> SpwanPrefabsHard;

    [SerializeField]
    protected List<GameObject> SpwanPrefabsInsane;

    public GameObject GetEnemySpawnByDifficulty(RoomDifficulty difficulty)
    {
        switch (difficulty)
        {
            case RoomDifficulty.Easy:
                return SpwanPrefabsEasy[RandomGenerator.SeededRange(0, SpwanPrefabsEasy.Count-1)];
            case RoomDifficulty.Medium:
                return SpwanPrefabsMedium[RandomGenerator.SeededRange(0, SpwanPrefabsMedium.Count - 1)];
            case RoomDifficulty.Hard:
                return SpwanPrefabsHard[RandomGenerator.SeededRange(0, SpwanPrefabsHard.Count - 1)];
            case RoomDifficulty.Insane:
                return SpwanPrefabsInsane[RandomGenerator.SeededRange(0, SpwanPrefabsInsane.Count - 1)];
            default:
                TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "difficulty Type not found " + difficulty.ToString());
                return SpwanPrefabsEasy[0];
        }
    }

 }
