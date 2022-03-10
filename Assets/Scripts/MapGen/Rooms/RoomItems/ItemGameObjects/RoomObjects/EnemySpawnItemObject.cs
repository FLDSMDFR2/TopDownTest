using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnItemObject : RoomItemObject
{
    protected RoomDifficulty difficulty = RoomDifficulty.Easy;
    protected int maxEnemysToSpawn;
    protected int enemysSpawned;
    protected float maxTime = 5f;
    protected float minTime = .5f;

    protected virtual void Start()
    {
        StartCoroutine(CheckForSpawn());
    }

    protected override void GenerateConfigValues()
    {
        maxEnemysToSpawn = 1;
    }

    #region Logic
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
            var enemy = GOPoolManager.GetObject(enemyScript.GetPoolId(), enemyPrefab, transform.position, transform.rotation);

            var bEnemy = enemy.GetComponent<BaseEnemy>();
            if (bEnemy == null)
                return;
            bEnemy.InitalizeHealth();
        }

        enemysSpawned += 1;
    }
    #endregion
}
