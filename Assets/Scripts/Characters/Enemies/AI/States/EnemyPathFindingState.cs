using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyPathFindingState : EnemyBaseState
{
    /// <summary>
    /// Rate to recheck for new path, -1 to disable
    /// </summary>
    public float PathCheckRate = -1;

    /// <summary>
    /// Last time we checked the path
    /// </summary>
    protected float lastPathCheck;
    /// <summary>
    /// Current Index of path we are traveling to
    /// </summary>
    protected int currentPathIndex = -1;
    /// <summary>
    /// If we are waiting for path finding to return new path
    /// </summary>
    protected bool waitForPath;

    #region State Logic
    /// <summary>
    /// Find a path from current pos to supplied dest location
    /// </summary>
    /// <param name="destLoc">location to find a path to</param>
    protected virtual void FindPath(float3 destLoc)
    {
        lastPathCheck = Time.time;

        //Update path details
        AI.pathDtl.StartPos = AI.transform.position;
        AI.pathDtl.EndPos = destLoc;
        AI.pathDtl.UpdatePath = true;
        PathFindingPool.AddObjectForPathFinding(AI.gameObject, AI.pathDtl);

        currentPathIndex = 0;
        waitForPath = true;
    }

    /// <summary>
    /// Find a path and then move along the path
    /// </summary>
    /// <param name="destLoc"></param>
    protected virtual void FindAndMoveOnPath(float3 destLoc)
    {
        if (AI.pathDtl.Path == null || currentPathIndex < 0 || (PathCheckRate >= 0 && Time.time >= lastPathCheck + PathCheckRate))
        {
            // find a path to supplied dest if we dont already have a path 
            // if we are performing this at timed interval lastPathCheck > 0 then check if we need to recalculate the path
            FindPath(destLoc);
        }
        else if (waitForPath && !AI.pathDtl.UpdatePath)
        {
            // we are waiting for path and it is now updated
            AI.pathDtl = PathFindingPool.GetObjectForPathFinding(AI.gameObject);
            waitForPath = false;
        }
        else if (currentPathIndex >= 0 && currentPathIndex < AI.pathDtl.Path.Count)
        {
            // walk the path 
            var nextLoc = new float3(AI.pathDtl.Path[currentPathIndex].x, AI.transform.position.y, AI.pathDtl.Path[currentPathIndex].y);
            if (ArrivedAtLoc(nextLoc))
            {
                currentPathIndex++;
            }
            else
            {
                // set look to the final dest location and move to the next location on the path
                AI.Controller.SetLookLocation(destLoc);
                AI.Controller.SetMoveDirection(-((float3)AI.transform.position - nextLoc));
            }
        }
        else
        {
            // we have a path and we are at the last location on it
            AtEndOfPath();
        }
    }

    /// <summary>
    /// End of the path reached
    /// </summary>
    protected virtual void AtEndOfPath() { }
    #endregion

    #region Helpers
    /// <summary>
    /// Check if we are ~at a location
    /// </summary>
    /// <param name="nextLoc"></param>
    /// <returns></returns>
    protected virtual bool ArrivedAtLoc(Vector3 nextLoc)
    {
        // check if we are close enough to the location to count as arrived
        var mag = (AI.transform.transform.position - nextLoc).magnitude;
        return mag < 0.1f;
    }
    #endregion
}
