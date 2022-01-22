using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
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

        // TODO: change this to be more dynamic
        // just stop and look at the target
        float3 moveToPos = (AI.transform.position - AI.Enemy.GetTarget().position).normalized * AI.Enemy.AttackingDistance + AI.Enemy.GetTarget().position;
        AI.Controller.SetMoveDirection(-((float3)AI.transform.position - moveToPos));

        AI.Controller.SetLookLocation(AI.Enemy.GetTarget().transform.position);



        // tell enemy to attack
        AI.Enemy.Attack();
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
