using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    protected bool DebugEnabled = false;

    [Header("Config")]
    /// <summary>
    /// Radius for area we will still detect a target
    /// </summary>
    public float DetectRadius;
    /// <summary>
    /// Radius for when we will see a target
    /// </summary>
    public float LOSRadius;
    /// <summary>
    /// Angle for the field of view on the line of site
    /// </summary>
    [Range(0, 360)]
    public float LOSAngle;

    /// <summary>
    /// Target layers we are looking for
    /// </summary>
    public LayerMask TargetMask;
    /// <summary>
    /// Layers that obstruct the view
    /// </summary>
    public LayerMask ObstructionMask;

    [Header("RunTime Assigned")]
    /// <summary>
    /// Target we have found.. if there is one
    /// </summary>
    public Transform Target;
    /// <summary>
    /// If we can see the target and its in range to detect
    /// </summary>
    public bool CanDetect;
    /// <summary>
    /// If line of site is not blocked to the target
    /// </summary>
    public bool CanSee;
    /// <summary>
    /// If we have seen a target and it is now out of line of site but still in the DetectRadius
    /// </summary>
    protected bool trackingTarget;

    #region Debug
    void OnDrawGizmos()
    {
        if (DebugEnabled)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, LOSRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, DetectRadius);

            Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -LOSAngle / 2);
            Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, LOSAngle / 2);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + viewAngle01 * LOSRadius);
            Gizmos.DrawLine(transform.position, transform.position + viewAngle02 * LOSRadius);
        }
    }
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion

    /// <summary>
    /// Perform check for if target is in our Field of view
    /// </summary>
    /// <returns></returns>
    public virtual bool PerformFOVCheck()
    {
        // check if target has entered LOS
        if (PerformLOSCheck())
        {
            // start tracking target and return that we can see the target
            trackingTarget = true;
            return true;
        }

        // if we are tracking the target but it has left LOS lets check for if its still in Detection Radius
        if (trackingTarget && PerformDetectionCheck())
        {
            return true;
        }
        else
        {
            // we have lost the target
            trackingTarget = false;
        }

        return false;
    }

    /// <summary>
    /// If a target is in line of site
    /// </summary>
    /// <returns></returns>
    protected virtual bool PerformLOSCheck()
    {
        // get object in range
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, LOSRadius, TargetMask);

        if (rangeChecks.Length != 0)
        {
            // get first object in list...closest?
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // if found object is inside the view angle 
            if (Vector3.Angle(transform.forward, directionToTarget) < LOSAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // target is inside view range check for obstructions
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, ObstructionMask))
                {
                    Target = target;
                    CanSee = true;
                    CanDetect = true;
                }
                else
                {
                    CanSee = false;
                }
            }
            else
            {
                CanSee = false;
            }
        }
        else if (CanSee)
        {
            CanSee = false;
        }

        // if we cant see anything clear target
        if (!CanSee)
        {
            Target = null;
        }

        return CanSee;
    }

    /// <summary>
    /// If a target is in detection range
    /// </summary>
    /// <returns></returns>
    protected virtual bool PerformDetectionCheck()
    {
        // get object in range
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, DetectRadius, TargetMask);

        // if we have found something
        if (rangeChecks.Length != 0)
        {
            Target = rangeChecks[0].transform;
            CanDetect = true;
        }
        else
        {
            CanDetect = false;
        }

        return CanDetect;
    }

}