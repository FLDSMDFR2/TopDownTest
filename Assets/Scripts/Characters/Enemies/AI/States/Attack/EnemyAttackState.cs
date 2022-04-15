using Unity.Mathematics;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    protected float3 unassigned = new float3(-1, -1, -1);
    protected float3 attackingPos = new float3(-1, -1, -1);

    protected float waitingStartTime;
    protected float timeToWait;
    protected bool waitToMove = false;

    #region State Change
    /// <summary>
    /// Find the next state based on current status
    /// </summary>
    /// <returns></returns>
    protected override EnemyStates FindNextState()
    {
        if (!AI.Enemy.HasTarget())
        {
            ChangingStates();
            return EnemyStates.Wander;
        }
        else if (CheckForChaseStateChange())
        {
            ChangingStates();
            return EnemyStates.Chase;
        }

        return EnemyStates.Attack;
    }

    /// <summary>
    /// Check if we need to switch to chase state
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckForChaseStateChange()
    {
        //if we have a target but cant see it then switch back to chasing
        return (AI.Enemy.HasTarget() && !IsInSite());
    }

    /// <summary>
    /// If we are changing out of this state then do any clean up
    /// </summary>
    protected override void ChangingStates()
    {
        attackingPos = unassigned;
    }
    #endregion

    #region State Logic
    /// <summary>
    /// Perfrom this status logic
    /// </summary>
    protected override void PerformStateLogic()
    {
        // find pos at attacking distance from target
        float3 moveToPos = FindPosition();

        //add checks to keep spacing between other enemies
        float3 pos = (moveToPos + (float3)KeepSpacing());
        pos.y = 0;

        // if we are not at pos we are moving to then keep moving to it
        AI.Controller.SetMoveDirection(-((float3)AI.transform.position - pos));

        // look at target
        AI.Controller.SetLookLocation(AI.Enemy.GetTarget().transform.position);

        // tell enemy to attack
        AI.Enemy.Attack();
    }
    /// <summary>
    /// Find attacking pos
    /// </summary>
    /// <returns></returns>
    protected virtual float3 FindPosition()
    { 
        return attackingPos;
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Check to see if we need to keep spacing between us and another enemy
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 KeepSpacing()
    {
        Vector3 sum = Vector3.zero;
        float count = 0f;
        var mask = LayerMask.GetMask(Layers.Enemy.ToString());
        Collider[] rangeChecks = Physics.OverlapSphere(AI.transform.position, AI.Enemy.DistanceFromOtherEnemies, mask);
        if (rangeChecks.Length != 0)
        {
            foreach (var enemy in rangeChecks)
            {
                if (enemy.transform != AI.transform)
                {
                    Vector3 difference = AI.transform.position - enemy.transform.position;
                    difference = difference.normalized / Mathf.Abs(difference.magnitude);
                    sum += difference;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            // average the direction
            sum /= count;

            // set the speed of movement
            sum = sum.normalized * 1;

        }

        return sum;
    }

    /// <summary>
    /// If target is insite and we can see it
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsInSite()
    {
        AI.FOV.PerformFOVCheck();
        return AI.FOV.CanSee;
    }

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
