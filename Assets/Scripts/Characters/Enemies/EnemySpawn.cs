using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    protected Queue<GameObject> despawnedEnemies = new Queue<GameObject>();

    public int MaxEnemysToSpawn;
    protected int EnemysSpawned;
    protected int EnemysDeSpawned;

    public float MaxTime = .3f;
    public float MinTime = .01f;

    protected float time;
    protected float spawnTime;

    public GameObject EnemyPrefab;


    private void Awake()
    {
        var bEnemy = EnemyPrefab.GetComponent<BaseEnemy>();
        if (bEnemy == null)
        {
            //error
        }
    }

    void Start()
    {
        SetRandomTime();
        time = MinTime;
    }

    void SetRandomTime()
    {
        spawnTime = Random.Range(MinTime, MaxTime);
    }

    void Update()
    {

        if (EnemysSpawned >= MaxEnemysToSpawn)
        {
            //if all enemys we have spawned are dead we can destry this gameobject
            if (EnemysDeSpawned >= EnemysSpawned)
                Destroy(gameObject);

            return;
        }
            

        time += Time.deltaTime;

        //Check if its the right time to spawn the object
        if (time >= spawnTime)
        {
            SpawnEnemy();
            SetRandomTime();
        }    
    }

    protected virtual void SpawnEnemy()
    {
        if(despawnedEnemies.Count > 0)
        {
            var enemy = despawnedEnemies.Dequeue();
            var bEnemy = enemy.GetComponent<BaseEnemy>();
            if (bEnemy == null)
                return;

            bEnemy.InitalizeHealth();
            enemy.transform.position = transform.position;
            enemy.SetActive(true);
        }
        else
        {
            var enemy = Instantiate(EnemyPrefab, transform.position, transform.rotation, transform);
            var bEnemy = enemy.GetComponent<BaseEnemy>();
            if (bEnemy == null)
                return;
            bEnemy.SetSpwanManager(this);
        }

        time = 0;
        EnemysSpawned += 1;

    }

    public virtual void DeSpawnEnemy(GameObject enemy)
    {
        var bEnemy = enemy.GetComponent<BaseEnemy>();
        if (bEnemy == null)
            return;

        enemy.SetActive(false);
        despawnedEnemies.Enqueue(enemy);
        EnemysDeSpawned += 1;
    }

}
