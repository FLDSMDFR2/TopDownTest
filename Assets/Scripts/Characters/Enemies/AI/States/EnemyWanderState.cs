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
    protected float3 searchLocationDest = new float3(-1, -1, -1);
    /// <summary>
    /// Count we are on for looking differnt directions once we have reached the search location
    /// </summary>
    protected int searchLookCount;
    /// <summary>
    /// Location to look at when stopped and looking
    /// </summary>
    protected Vector3 searchLook = Vector3.zero;

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
        searchLocationDest = unassignedDest;
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
        if (searchLocationDest.Equals(unassignedDest))
        {
            // find new dest
            while (!GameManager.Instance.GetGridPath.Passable(searchLocationDest))
            {
                searchLocationDest = new float3(UnityEngine.Random.Range(AI.transform.position.x - AI.SearchRadius, AI.transform.position.x + AI.SearchRadius),
                   AI.transform.position.y,
                   UnityEngine.Random.Range(AI.transform.position.z - AI.SearchRadius, AI.transform.position.z + AI.SearchRadius));
            }

            //find path here
            FindPath(searchLocationDest);
        }
        else
        {
            // we have a dest so lets find a path and move to it
            FindAndMoveOnPath(searchLocationDest);
        }

        // look to see if we can see the player and the player is in range of LOS
        if (AI.FOV.PerformFOVCheck() && AI.FOV.CanDetect)
        {
            AI.Enemy.SetTarget(AI.FOV.Target);
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
            searchLocationDest = unassignedDest;
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
