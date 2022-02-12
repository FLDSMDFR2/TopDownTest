using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class RoomItemEnemySpawn : RoomItem
{
    protected int distanceFromWalls = 5;

    protected RoomDifficulty difficulty = RoomDifficulty.Easy;
    protected int maxEnemysToSpawn;
    protected int enemysSpawned;
    protected float maxTime = 5f;
    protected float minTime = .5f;

    #region Config
    protected override void ConfigureSelf()
    {
        Type = RoomItemType.EnemySpwan;
        maxItemUsesInRoom = 1;
        maxEnemysToSpawn = 5;
    }
    #endregion

    #region Item Logic
    public override void Start()
    {
        RoomItemMonoBehaviour.StartCoroutine(CheckForSpawn());
    }

    protected virtual IEnumerator CheckForSpawn()
    {
        while (enemysSpawned < maxEnemysToSpawn)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minTime, maxTime));

            SpawnEnemy();

            yield return null;
        }
    }

    protected virtual void SpawnEnemy()
    {
        var enemyPrefab = EnemySpawnManager.GetEnemySpawnByDifficulty(difficulty);
        var enemyScript = enemyPrefab.GetComponent<BaseEnemy>();
        if (enemyScript != null)
        {
            var enemy = GOPoolManager.GetObject(enemyScript.GetPoolId(), enemyPrefab, RoomItemMonoBehaviour.transform.position, RoomItemMonoBehaviour.transform.rotation);

            var bEnemy = enemy.GetComponent<BaseEnemy>();
            if (bEnemy == null)
                return;
            bEnemy.InitalizeHealth();
        }

        enemysSpawned += 1;
    }
    #endregion

    #region Place Location Checks
    protected override bool CheckLocation(Room room, int2 locationKey)
    {
        // check that this location is x Distance from walls in all directions
        foreach (var dir in MapTraversal.NeighborsDirectionsAll)
        {
            var checkLoc = locationKey;
            for (int i = 0; i <= distanceFromWalls; i++)
            {
                checkLoc += dir;
                if (room.Data.RoomLocations.ContainsKey(checkLoc) && room.Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
                    return false; // location is to close to a wall
                else if (!room.Data.RoomLocations.ContainsKey(checkLoc))
                    return false; // location is to close to edge of room
            }
        }

        return true;
    }
    #endregion
}
