
public class EnemyAIRanged : EnemyAI
{
    protected override void InitStates()
    {
        EnemyWander = new EnemyWanderState();
        EnemyChase = new EnemyChaseState();
        EnemyAttack = new EnemyAttackStateRanged();

        EnemyWander.PathCheckRate = WanderPathCheckRate;
        EnemyChase.PathCheckRate = ChasePathCheckRate;
    }
}
