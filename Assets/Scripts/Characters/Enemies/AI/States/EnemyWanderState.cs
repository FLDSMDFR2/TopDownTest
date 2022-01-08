using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWanderState : IEnemyStates
{
    protected EnemyAI AI;

    protected List<Vector2Int> path = null;
    protected int pathIndex = -1;
    protected PathFinding pathFinder = new PathFinding();

    public EnemyStates PerformState(EnemyAI ai)
    {
        AI = ai;

        PerformSearchLogic();

        return FindNextState();
    }

    public EnemyStates FindNextState()
    {
        if (AI.Enemy.HasTarget())
        {
            pathIndex = -1;
            return EnemyStates.Chase;
        }
        else
        {
            return EnemyStates.Wander;
        }
    }

    protected virtual void PerformSearchLogic()
    {
        if (path == null || pathIndex < 0)
        {
            FindPath();
        }
        else if (pathIndex < path.Count)
        {
            var nextLoc = new Vector3(path[pathIndex].x, AI.transform.position.y, path[pathIndex].y);
            if (AI.transform.transform.position.Equals(nextLoc))
            {
                pathIndex++;
            }
            else
            {
                AI.transform.transform.position = Vector3.MoveTowards(AI.transform.transform.position, nextLoc, AI.Controller.CurrentMoveSpeed * Time.deltaTime);
            }
        }
        else
        {
            FindPath();
        }

        AI.Enemy.SetTarget(AI.SearchZone.CheckTarget());

    }

    protected virtual void FindPath()
    {
        //find valid next loc
        Vector2Int tempNext = new Vector2Int(Int32.MinValue, Int32.MinValue);
        while (!GameManager.Instance.GetGridPath.Passable(tempNext))
        {
            tempNext = pathFinder.Round(new Vector3(UnityEngine.Random.Range(AI.transform.position.x - AI.SearchDetectRadius, AI.transform.position.x + AI.SearchDetectRadius),
               AI.transform.position.y,
               UnityEngine.Random.Range(AI.transform.position.z - AI.SearchDetectRadius, AI.transform.position.z + AI.SearchDetectRadius)));
        }

        // find a path to the location
        path = pathFinder.GetFindPath(GameManager.Instance.GetGridPath, AI.transform.position, new Vector3(tempNext.x, AI.transform.position.y, tempNext.y));

        if (path.Count > 0)
            pathIndex = 0;
        else
            pathIndex = -1;
    }

    protected virtual bool CheckForAttack()
    {
        var target = AI.SearchZone.CheckTarget();
        AI.Enemy.SetTarget(target);

        if (!AI.Enemy.HasTarget())
            return false;

        AI.transform.LookAt(target);

        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(AI.transform.position, AI.transform.TransformDirection(Vector3.forward));
        if (Physics.Raycast(ray, out hit, AI.SearchDetectRadius, LayerMask.NameToLayer("Projectile")))
        {
            var player = hit.collider.gameObject.GetComponent<BasePlayer>();
            if (player == null)
            {
                AI.Enemy.SetTarget(null);
                return false;
            }
        }

        return true;
    }
}
