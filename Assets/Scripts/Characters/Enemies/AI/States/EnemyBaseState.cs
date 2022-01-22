using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState : IEnemyStates
{
    /// <summary>
    /// AI manager class
    /// </summary>
    protected EnemyAI AI;

    /// <summary>
    /// Perform this states functunality 
    /// </summary>
    /// <param name="ai"></param>
    /// <returns>next state to be in after performing this state</returns>
    public virtual EnemyStates PerformState(EnemyAI ai)
    {
        AI = ai;

        PerformStateLogic();

        return FindNextState();
    }

    #region State Change
    /// <summary>
    /// Find the next state based on current status
    /// </summary>
    /// <returns></returns>
    protected virtual EnemyStates FindNextState()
    {
        return EnemyStates.None;
    }

    /// <summary>
    /// If we are changing out of this state then do any clean up
    /// </summary>
    protected virtual void ChangingStates() { }
    #endregion

    #region State Logic

    /// <summary>
    /// Perfrom this status logic
    /// </summary>
    protected virtual void PerformStateLogic() { }
    #endregion
}