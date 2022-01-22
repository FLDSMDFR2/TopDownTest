using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyWanderState : IEnemyStates
{
    protected EnemyAI AI;
    protected int pathIndex = -1;
    protected bool waitForPath;

    protected int searchLocations;

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
        if (AI.pathDtl.Path == null || pathIndex < 0)
        {
            FindPath();
        }
        else if (waitForPath && !AI.pathDtl.UpdatePath)
        {
            // we are waiting for path and it is now updated
            AI.pathDtl = PathFindingPool.GetObjectForPathFinding(AI.gameObject);
            waitForPath = false;
        }
        else if (pathIndex < AI.pathDtl.Path.Count)
        {
            var nextLoc = new Vector3(AI.pathDtl.Path[pathIndex].x, AI.transform.position.y, AI.pathDtl.Path[pathIndex].y);
            if (ArrivedAtLoc(nextLoc))
            {
                pathIndex++;
            }
            else
            {
                // look and move to the next location
                AI.Controller.SetLookLocation(nextLoc);
                AI.Controller.SetMoveDirection(-(AI.gameObject.transform.position - nextLoc));
            }
        }
        else
        {
            AtSearchLocation();
        }

        // look to see if we can see the player and the player is in range of LOS
        if(AI.FOV.PerformFOVCheck() && AI.FOV.CanDetect)
        {
            AI.Enemy.SetTarget(AI.FOV.Target);
        }
    }

    protected virtual void AtSearchLocation()
    {
        AI.Controller.SetMoveDirection(Vector3.zero);
        var searchLook = Vector3.zero;
        if (searchLocations == 0)
        {
            var x = 1 * Mathf.Cos(30 * Mathf.Deg2Rad);
            var y = 1 * Mathf.Sin(30 * Mathf.Deg2Rad);
            searchLook = AI.transform.position;
            searchLook.x += x;
            searchLook.z += y;
        }
        else if (searchLocations == 1)
        {
            var x = 1 * Mathf.Cos(-60 * Mathf.Deg2Rad);
            var y = 1 * Mathf.Sin(-60 * Mathf.Deg2Rad);
            searchLook = AI.transform.position;
            searchLook.x += x;
            searchLook.z += y;
        }

        AI.Controller.SetLookLocation(searchLook);
        float dot = Vector3.Dot(AI.transform.forward, (searchLook - AI.transform.position).normalized);
        if (dot == 1)
        {
            searchLocations++;
        }
        
        if (searchLocations == 2)
        {
            searchLocations = 0;
            FindPath();
        }
    }

    protected virtual bool ArrivedAtLoc(Vector3 nextLoc)
    {
        // check if we are close enough to the location to count as arrived
        var mag = (AI.transform.transform.position - nextLoc).magnitude;
        return mag < 0.1f;
    }

    protected virtual void FindPath()
    {
        //find valid next loc
        Vector3 next = new Vector3(-1,-1,-1);
        while (!GameManager.Instance.GetGridPath.Passable(next))
        {
            next = new Vector3(UnityEngine.Random.Range(AI.transform.position.x - AI.SearchRadius, AI.transform.position.x + AI.SearchRadius),
               AI.transform.position.y,
               UnityEngine.Random.Range(AI.transform.position.z - AI.SearchRadius, AI.transform.position.z + AI.SearchRadius));
        }

        //Update path details
        AI.pathDtl.StartPos = AI.transform.position;
        AI.pathDtl.EndPos = next;
        AI.pathDtl.UpdatePath = true;
        PathFindingPool.AddObjectForPathFinding(AI.gameObject, AI.pathDtl);

        pathIndex = 0;
        waitForPath = true;
    }
}
