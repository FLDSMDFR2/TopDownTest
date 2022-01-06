using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : IEnemyStates
{
    protected EnemyAI AI;

    protected List<Vector2Int> path = null;
    protected int pathIndex = -1;
    protected float lastPathCheck;
    protected PathFinding pathFinder = new PathFinding();

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

        if (path == null || Time.time >= lastPathCheck + AI.PathCheckRate)
        {
            FindPath();
        }
        else if (pathIndex >= 0 && pathIndex < path.Count)
        {
            var nextLoc = new Vector3(path[pathIndex].x, AI.transform.position.y, path[pathIndex].y);
            if (AI.transform.transform.position.Equals(nextLoc))
            {
                pathIndex++;
            }
            else
            {
                var nextPos = new Vector3(path[0].x, AI.transform.position.y, (path[0].y));
                AI.transform.transform.position = Vector3.MoveTowards(AI.transform.transform.position, nextPos, AI.Controller.CurrentSpeed * Time.deltaTime);
            }
        }

        AI.Enemy.SetTarget(AI.SearchZone.CheckTarget());
    }

    protected virtual void FindPath()
    {
        lastPathCheck = Time.time;

        // find a path to the location
        path = pathFinder.GetFindPath(GameManager.Instance.GetGridPath, AI.transform.position, AI.Enemy.GetTarget().position);

        if (path.Count > 0)
            pathIndex = 0;
        else
            pathIndex = -1;
    }
}
