using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class EnemyAIUpdateManager : MonoBehaviour
{
    [SerializeField]
    private List<EnemyAI> enemyAIs;

    //private void Update()
    //{
    //    var enemyAIDataArray = new NativeArray<EnemyAI.Data>(enemyAIs.Count, Allocator.TempJob);
    //    for (var i = 0; i < enemyAIs.Count; i++)
    //    {
    //        enemyAIDataArray[i] = new EnemyAI.Data(enemyAIs[i]);
    //    }
    //    var job = new EnemyAIUpdateJob
    //    {
    //        EnemyAIDataArray = enemyAIDataArray
    //    };
    //    var jobHandle = job.Schedule(enemyAIs.Count, 1);
    //    jobHandle.Complete();
    //    enemyAIDataArray.Dispose();
    //}
}
