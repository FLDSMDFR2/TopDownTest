public interface IEnemyStates
{
    EnemyStates PerformState(EnemyAI ai);

    EnemyStates FindNextState();
}
