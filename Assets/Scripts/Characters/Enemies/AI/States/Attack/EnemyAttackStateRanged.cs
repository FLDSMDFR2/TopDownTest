using Unity.Mathematics;
using UnityEngine;

public class EnemyAttackStateRanged : EnemyAttackState
{
    #region State Logic
    protected override float3 FindPosition()
    {
        if (ArrivedAtLoc(attackingPos) && !waitToMove)
        {
            waitToMove = true;
            waitingStartTime = Time.time;
            timeToWait = RandomGenerator.RandomRange(1, 4);
        }
        else if (waitToMove && Time.time >= waitingStartTime + timeToWait)
        {
            waitToMove = false;
            attackingPos = unassigned;
        }

        if (attackingPos.Equals(unassigned) || Vector3.Distance(AI.transform.position, AI.Enemy.GetTarget().position) < AI.Enemy.AttackingDistance - 3)
        {
            var distance = RandomGenerator.RandomRange(AI.Enemy.AttackingDistance, AI.Enemy.AttackRange);
            var offsetX = RandomGenerator.RandomRange(-0.1f, 0.1f);
            var offsetZ = RandomGenerator.RandomRange(-0.1f, 0.1f);
            attackingPos = ((AI.transform.position - AI.Enemy.GetTarget().position).normalized + new Vector3(offsetX, 0, offsetZ)) * distance + AI.Enemy.GetTarget().position;
        }

        return attackingPos;
    }
    #endregion

}
