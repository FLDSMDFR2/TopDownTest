
public class EnemyAIClose : EnemyAI
{
    protected override void InitStates()
    {
        EnemyWander = new EnemyWanderState();
        EnemyChase = new EnemyChaseState();
        EnemyAttack = new EnemyAttackStateClose();

        EnemyWander.PathCheckRate = WanderPathCheckRate;
        EnemyChase.PathCheckRate = ChasePathCheckRate;
    }
}
