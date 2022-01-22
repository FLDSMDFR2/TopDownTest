using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyChaseState : IEnemyStates
{
    protected EnemyAI AI;

    protected int pathIndex = -1;
    protected float lastPathCheck;
    protected bool waitForPath;

    public EnemyStates PerformState(EnemyAI ai)
    {
        AI = ai;

        AI.Controller.IsChase = true;

        MoveToTarget();

        return FindNextState();
    }

    public EnemyStates FindNextState()
    {
        if (!AI.Enemy.HasTarget())
        {
            pathIndex = -1;
            AI.Controller.IsChase = false;
            return EnemyStates.Wander;
        }
        else
        {
            return EnemyStates.Chase;
        }
    }

    protected virtual void MoveToTarget()
    {
        if (IsInSite())
        {
            // just move directly at the target
            AI.Controller.SetLookLocation(AI.Enemy.GetTarget().transform.position);
            AI.Controller.SetMoveDirection(-(AI.gameObject.transform.position - AI.Enemy.GetTarget().transform.position));
        }
        else
        {
            // find path to the target as its not insite
            MoveAlongPath();
        }

        AI.Enemy.SetTarget(AI.FOV.Target);
    }

    protected virtual bool IsInSite()
    {
        AI.FOV.PerformFOVCheck();
        return AI.FOV.CanSee;
    }

    protected virtual void MoveAlongPath()
    {
        if (AI.pathDtl.Path == null || pathIndex < 0 || Time.time >= lastPathCheck + AI.PathCheckRate)
        {
            FindPath();
        }
        else if (waitForPath && !AI.pathDtl.UpdatePath)
        {
            // we are waiting for path and it is now updated
            AI.pathDtl = PathFindingPool.GetObjectForPathFinding(AI.gameObject);
            waitForPath = false;
        }
        else if (pathIndex >= 0 && pathIndex < AI.pathDtl.Path.Count)
        {
            var nextLoc = new Vector3(AI.pathDtl.Path[pathIndex].x, AI.transform.position.y, AI.pathDtl.Path[pathIndex].y);
            if (ArrivedAtLoc(nextLoc))
            {
                pathIndex++;
            }
            else
            {
                AI.Controller.SetLookLocation(AI.Enemy.GetTarget().position);
                AI.Controller.SetMoveDirection(-(AI.gameObject.transform.position - nextLoc));
            }
        }
    }

    protected virtual void FindPath()
    {
        lastPathCheck = Time.time;

        AI.pathDtl.StartPos = AI.transform.position;
        AI.pathDtl.EndPos = AI.Enemy.GetTarget().position;
        AI.pathDtl.UpdatePath = true;
        PathFindingPool.AddObjectForPathFinding(AI.gameObject, AI.pathDtl);

        pathIndex = 0;
        waitForPath = true;
    }

    protected virtual bool ArrivedAtLoc(Vector3 nextLoc)
    {
        // check if we are close enough to the location to count as arrived
        var mag = (AI.transform.transform.position - nextLoc).magnitude;
        return mag < 0.1f;
    }
}
