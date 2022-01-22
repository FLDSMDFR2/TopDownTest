using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseWeapon : BaseWeapon
{
    protected override System.Type GetProjectileType()
    {
        return typeof(EnemyBaseProjectile);
    }
}
