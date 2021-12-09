using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Vector3 Location;
    public RoomDifficulty difficulty = RoomDifficulty.Easy;
    public int MaxEnemysToSpawn;
    public float MaxTime = 5f;
    public float MinTime = .5f;

    protected int enemysSpawned;

    void Start()
    {
        StartCoroutine(CheckForSpawn());
    }

    protected virtual IEnumerator CheckForSpawn()
    {
        while (enemysSpawned < MaxEnemysToSpawn)
        {
            yield return new WaitForSeconds(Random.Range(MinTime, MaxTime));

            SpawnEnemy();

            yield return null;
        }
    }

    protected virtual void SpawnEnemy()
    {
        var enemyPrefab = EnemySpawnManager.GetEnemySpawnByDifficulty(difficulty);
        var enemy = GOPoolManager.GetObject(typeof(BaseEnemy), enemyPrefab, Location, transform.rotation);

        var bEnemy = enemy.GetComponent<BaseEnemy>();
        if (bEnemy == null)
            return;
        bEnemy.InitalizeHealth();

        enemysSpawned += 1;
    }
}
