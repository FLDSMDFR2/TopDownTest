using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseCharacter
{
    [SerializeField]
    protected Transform _target;

    public void SetTarget(Transform target)
    {
        _target = target;
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

        if (ActiveWeapon != null)
        {
            FireWeapon();
        }
    }

    protected override void Dead()
    {
        base.Dead();
        GOPoolManager.AddObject(this.GetType(), gameObject);
    }
}
