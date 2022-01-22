using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyInputController : BaseInputController
{
    [Header("EnemyInputController")]
    /// <summary>
    /// Chase movement speed
    /// </summary>
    public float ChaseMoveSpeed = 15f;

    protected bool isChase;
    public bool IsChase
    {
        get { return isChase; }
        set
        {
            isChase = value;
            UpdatesForChase();
        }
    }


    /// <summary>
    /// Set the direction to move in
    /// </summary>
    /// <param name="moveDir"></param>
    public virtual void SetMoveDirection(float3 moveDir)
    {
        // set the move direction for enemy
        moveDirection = math.normalizesafe(moveDir);
    }

    /// <summary>
    /// Set location for enemy to look at
    /// </summary>
    /// <param name="lookLoc"></param>
    public virtual void SetLookLocation(float3 lookLoc)
    {
        // set the location to look at
        lookLocation = lookLoc;
    }

    /// <summary>
    /// Set the chase speed
    /// </summary>
    /// <param name="speed"></param>
    public virtual void SetChaseSpeed(float speed)
    {
        ChaseMoveSpeed = speed;
    }
    /// <summary>
    /// If we are chasing 
    /// </summary>
    protected virtual void UpdatesForChase()
    {
        if (IsChase)
        {
            CurrentMoveSpeed = ChaseMoveSpeed;
        }
        else
        {
            CurrentMoveSpeed = BaseMoveSpeed;
        }
    }
}
