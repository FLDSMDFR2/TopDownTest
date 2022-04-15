using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyWanderState : EnemyPathFindingState
{
    /// <summary>
    /// check value if we dont have a search dest location set
    /// </summary>
    protected float3 unassignedDest = new float3(-1, -1, -1);
    /// <summary>
    /// Destination we are traveling to to search it
    /// </summary>
    protected float3 unassigned = new float3(-1, -1, -1);
    /// <summary>
    /// Count we are on for looking differnt directions once we have reached the search location
    /// </summary>
    protected int searchLookCount;
    /// <summary>
    /// Location to look at when stopped and looking
    /// </summary>
    protected Vector3 searchLook = Vector3.zero;
    /// <summary>
    /// If we have a target in hearing range
    /// </summary>
    protected BaseCharacter hearingTarget;

    protected float lookMargin = .0001f;

    #region State Change
    /// <summary>
    /// Find the next state based on current status
    /// </summary>
    /// <returns></returns>
    protected override EnemyStates FindNextState()
    {
        if (AI.Enemy.HasTarget())
        {
            ChangingStates();
            return EnemyStates.Chase;
        }

        return EnemyStates.Wander;
    }
    /// <summary>
    /// If we are changing out of this state then do any clean up
    /// </summary>
    protected override void ChangingStates()
    {
        StopListening();
        unassigned = unassignedDest;
        currentPathIndex = -1;
    }
    #endregion

    #region State Logic
    /// <summary>
    /// Perfrom this status logic
    /// </summary>
    protected override void PerformStateLogic()
    {
        //find we dont have a searchdest
        if (unassigned.Equals(unassignedDest))
        {
            // find new dest
            while (!GameManager.Instance.GetGridPath.Passable(unassigned))
            {
                unassigned = new float3(RandomGenerator.RandomRange(AI.transform.position.x - AI.SearchRadius, AI.transform.position.x + AI.SearchRadius),
                   AI.transform.position.y,
                   RandomGenerator.RandomRange(AI.transform.position.z - AI.SearchRadius, AI.transform.position.z + AI.SearchRadius));
            }

            //find path here
            FindPath(unassigned);
        }
        else
        {
            // we have a dest so lets find a path and move to it
            FindAndMoveOnPath(unassigned);
        }

        // look to see if we can see the player and the player is in range of LOS
        if (AI.FOV.PerformFOVCheck() && AI.FOV.CanDetect)
        {
            AI.Enemy.SetTarget(AI.FOV.Target);
        }
        else if (AI.FOV.CanHear)
        {
            hearingTarget = AI.FOV.HearingTarget.GetComponent<BaseCharacter>();
            Listen();
        }
        else
        {
            StopListening();
        }
    }

    /// <summary>
    /// Perfor logic for once we have reach the end of the path
    /// </summary>
    protected override void AtEndOfPath()
    {
        // stop the enemy
        AI.Controller.SetMoveDirection(Vector3.zero);

        if (searchLookCount == 0 && searchLook == Vector3.zero)
        {
            //look right 60 degress
            searchLook = LookTo(AI.transform, 60, 1);
        }
        else if (searchLookCount == 1 && searchLook == Vector3.zero)
        {
            //look back left 120 degress so we look right / left 60 degress from start 
            searchLook = LookTo(AI.transform, -120, 1);
        }

        //update look to
        AI.Controller.SetLookLocation(searchLook);

        float dot = Vector3.Dot(AI.transform.forward, (searchLook - AI.transform.position).normalized);
        if (dot <= 1 + lookMargin && dot >= 1 - lookMargin)
        {
            searchLookCount++;
            searchLook = Vector3.zero;
        }

        if (searchLookCount == 2)
        {
            searchLookCount = 0;
            unassigned = unassignedDest;
        }
    }
    #endregion

    #region Listen
    /// <summary>
    /// Listen for target in hearing range
    /// </summary>
    protected virtual void Listen()
    {
        if (hearingTarget == null) return;

        StopListening();

        hearingTarget.ActiveWeapon.ShootingModifier.IsFiringEvent += HearingTarget_IsFiringEvent;
    }

    /// <summary>
    /// Stop listening for target
    /// </summary>
    protected virtual void StopListening()
    {
        if (hearingTarget == null) return;
        hearingTarget.ActiveWeapon.ShootingModifier.IsFiringEvent -= HearingTarget_IsFiringEvent;
    }

    /// <summary>
    /// Handle if target it firing 
    /// </summary>
    private void HearingTarget_IsFiringEvent()
    {
        if (AI.FOV.CanHear)
        {
            // set new search location to last fire postistion 
            unassigned = AI.FOV.HearingTarget.position;

            //find path here
            FindPath(unassigned);
        }
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Look to angle offset based on current location and facing
    /// </summary>
    /// <param name="pos">transfom of object to find look pos</param>
    /// <param name="angle">angle from current facing we want to look to</param>
    /// <param name="distance">distance we will look out</param>
    /// <returns>location we will look at</returns>
    protected virtual Vector3 LookTo(Transform pos, float angle, float distance)
    {
        // local coordinate rotation around the Y axis to the given angle
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        // add the desired distance to the direction
        Vector3 addDistanceToDirection = rotation * pos.forward * distance;

        // add the distance and direction to the current position to get the final destination
        return pos.position + addDistanceToDirection;
    }
    #endregion
}
