using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : IEnemyStates
{
    protected EnemyAI AI;

    public EnemyStates PerformState(EnemyAI ai)
    {
        AI = ai;

        MoveToTarget();

        return FindNextState();
    }

    public EnemyStates FindNextState()
    {
        if (!AI.Enemy.HasTarget())
        {
            return EnemyStates.Wander;
        }
        else
        {
            return EnemyStates.Chase;
        }
    }

    protected virtual void MoveToTarget()
    {
        AI.transform.LookAt(AI.Enemy.GetTarget());
        AI.Controller.UpdateEnemy();

        var path = AI.pathFinding.GetFindPath(GameManager.Instance.GetGridPath, AI.transform.position, AI.Enemy.GetTarget().position);

        if (path.Count > 0)
        {
            var nextPos = new Vector3(path[0].x, AI.transform.position.y, (path[0].y));
            AI.transform.transform.position = Vector3.MoveTowards(AI.transform.transform.position, nextPos, AI.Controller.CurrentSpeed * Time.deltaTime);
        }

        AI.Enemy.SetTarget(AI.SearchZone.CheckTarget());
    }
}
