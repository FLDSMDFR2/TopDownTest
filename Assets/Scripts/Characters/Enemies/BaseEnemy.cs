using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : BaseCharacter, IPoolable
{
    [Header("Base Enemy")]
    /// <summary>
    /// Id of this object for object pooling
    /// </summary>
    [SerializeField]
    protected string PoolingID;
    /// <summary>
    /// Range this enemy will start attacking within
    /// </summary>
    public float AttackRange;
    /// <summary>
    /// Distance enemy will try to stay from target while attacking
    /// </summary>
    public float AttackingDistance;
    /// <summary>
    /// Distance enemy will try to stay from other Enemies
    /// </summary>
    public float DistanceFromOtherEnemies;

    /// <summary>
    /// Current target of the enemy 
    /// </summary>
    protected Transform _target;

    /// <summary>
    /// Set the target
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    /// <summary>
    /// Get the current target can return null if has no target
    /// </summary>
    /// <returns></returns>
    public Transform GetTarget()
    {
        return _target;
    }

    /// <summary>
    /// If we have a current target
    /// </summary>
    /// <returns></returns>
    public virtual bool HasTarget()
    {
        return _target != null;
    }

    /// <summary>
    /// Perform attack
    /// </summary>
    public virtual void Attack()
    {
        if (_target == null)
            return;

        if (ActiveWeapon != null)
        {
            FireWeapon();
        }
    }

    /// <summary>
    /// Death handling
    /// </summary>
    protected override void Dead()
    {
        base.Dead();
        GOPoolManager.AddObject(GetPoolId(), gameObject);
    }

    public string GetPoolId()
    {
        return PoolingID;
    }
}
