using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyChaseState : EnemyPathFindingState
{
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
        else if (CheckForAttackStateChange())
        {
            ChangingStates();
            return EnemyStates.Attack;
        }

        return EnemyStates.Chase;
    }

    /// <summary>
    /// Check to see if we are going to switch to attacking
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckForAttackStateChange()
    {
        // if we have a target and we can see it
        if (AI.Enemy.HasTarget() && IsInSite())
        {
            // retrun if we are in range of attack or not
            return Vector3.Distance(AI.Enemy.GetTarget().position, AI.transform.position) < AI.Enemy.AttackRange;
        }
        
        return false;
    }

    /// <summary>
    /// If we are changing out of this state then do any clean up
    /// </summary>
    protected override void ChangingStates()
    {
        currentPathIndex = -1;
        AI.Controller.IsChase = false;
    }
    #endregion

    #region State Logic
    /// <summary>
    /// Perfrom this status logic
    /// </summary>
    protected override void PerformStateLogic()
    {
        // notify the movment controller we are chasing / attcking
        AI.Controller.IsChase = true;

        if (IsInSite())
        {
            // just move directly at the target
            AI.Controller.SetLookLocation(AI.Enemy.GetTarget().transform.position);
            AI.Controller.SetMoveDirection(-(AI.transform.position - AI.Enemy.GetTarget().transform.position));
        }
        else
        {
            // find path to the target as its not insite
            FindAndMoveOnPath(AI.Enemy.GetTarget().position);
        }

        AI.Enemy.SetTarget(AI.FOV.Target);
    }
    #endregion

    #region Helpers
    /// <summary>
    /// If target is insite and we can see it
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsInSite()
    {
        AI.FOV.PerformFOVCheck();
        return AI.FOV.CanSee;
    }
    #endregion
}
