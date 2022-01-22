using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseInputController : MonoBehaviour
{
    [Header("BaseInputController")]
    /// <summary>
    /// Base speed to move
    /// </summary>
    public float BaseMoveSpeed = 5f;
    /// <summary>
    /// Base speed to look
    /// </summary>
    public float BaseLookSpeed = 5f;
    /// <summary>
    /// Dash speed
    /// </summary>
    public float DashSpeed;
    /// <summary>
    /// Current moving speed
    /// </summary>
    public float CurrentMoveSpeed;
    /// <summary>
    /// Rigidbody to control
    /// </summary>
    protected Rigidbody body;
    /// <summary>
    /// Movement vector
    /// </summary>
    protected Vector3 moveDirection;

    public float LookOffSet = 90;
    /// <summary>
    /// Location to look at
    /// </summary>
    protected Vector3 lookLocation;
    /// <summary>
    /// Direction vector to look at
    /// </summary>
    protected Vector3 lookDirection;
    /// <summary>
    /// If we want to dash
    /// </summary>
    protected bool isDash;

    public void Awake()
    {
        //default current move speed to base speed on start
        CurrentMoveSpeed = BaseMoveSpeed;
        // get rigidbody we want to control
        body = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        //handle all movment input
        HandleMovementInput();

        //handle dash input
        HandleDashInput();

        //handle all looking input
        HandleLookInput();
    }

    protected virtual void FixedUpdate()
    {
        //perform the movement 
        PerformMovement();
        //perform the look
        PerformLook();
    }

    #region Handlers
    /// <summary>
    /// Handle input for movement
    /// </summary>
    protected virtual void HandleMovementInput(){}
    /// <summary>
    /// Handle Dash input
    /// </summary>
    protected virtual void HandleDashInput() {}
    /// <summary>
    /// Handle input to look
    /// </summary>
    protected virtual void HandleLookInput() {}
    #endregion

    #region Perform
    /// <summary>
    /// Perform movment
    /// </summary>
    protected virtual void PerformMovement()
    {
        if (isDash)
        {
            body.AddForce(moveDirection * DashSpeed, ForceMode.VelocityChange);
            isDash = false;
        }
        else
        {
            // noraml movement
            body.AddForce(moveDirection * CurrentMoveSpeed, ForceMode.Impulse);
        }

    }
    /// <summary>
    /// Perform Look
    /// </summary>
    protected virtual void PerformLook()
    {
        lookDirection = lookLocation - body.position;
        var angle = Mathf.Atan2(lookDirection.z, lookDirection.x) * Mathf.Rad2Deg - LookOffSet;
        if (BaseLookSpeed == 0)//instant
        {
            body.MoveRotation(Quaternion.Euler(0f, -angle, 0f));
        }
        else
        {
            body.MoveRotation(Quaternion.Lerp(body.rotation, Quaternion.Euler(0f, -angle, 0f), Time.fixedDeltaTime * BaseLookSpeed));
        }
    }
    #endregion
}
