using Unity.Mathematics;
using UnityEngine;

public class EnemyAttackStateClose : EnemyAttackState
{
    #region State Logic
    /// <summary>
    /// Perfrom this status logic
    /// </summary>
    protected override void PerformStateLogic()
    {
        // notify the movment controller we are chasing / attcking
        AI.Controller.IsChase = true;

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

    protected override float3 FindPosition()
    {
        if (attackingPos.Equals(unassigned) || Vector3.Distance(AI.transform.position, AI.Enemy.GetTarget().position) > AI.Enemy.AttackingDistance || ArrivedAtLoc(attackingPos))
        {
            var distance = RandomGenerator.RandomRange(AI.Enemy.AttackingDistance, AI.Enemy.AttackRange);
            attackingPos = ((AI.transform.position - AI.Enemy.GetTarget().position).normalized) * distance + AI.Enemy.GetTarget().position;
        }

        return attackingPos;
    }
    #endregion
}
