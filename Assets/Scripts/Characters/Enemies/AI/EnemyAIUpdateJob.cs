using Unity.Collections;
using Unity.Jobs;

public struct EnemyAIUpdateJob : IJobParallelFor
{
    public NativeArray<EnemyAI.Data> EnemyAIDataArray;

    public void Execute(int index)
    {
        var data = EnemyAIDataArray[index];
        //data.Update();
        EnemyAIDataArray[index] = data;
    }
}
