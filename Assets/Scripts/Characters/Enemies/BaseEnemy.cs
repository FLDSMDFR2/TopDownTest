using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseCharacter
{
    protected Transform _target;
    protected EnemySpawn _spwanManager;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetSpwanManager(EnemySpawn manager)
    {
        _spwanManager = manager;
    }

    public Transform GetTarget()
    {
        return _target;
    }

    public virtual bool HasTarget()
    {
        return _target != null;
    }

    public virtual void Attack()
    {
        if (_target == null)
            return;

        transform.LookAt(_target);

        if (PrimaryWeapon != null)
        {
            FirePrimaryWeapon();
        }
    }

    protected override void Dead()
    {
        base.Dead();
        if (_spwanManager == null)
            Destroy(gameObject);
        else
            _spwanManager.DeSpawnEnemy(gameObject);
    }
}
