using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseInputController : MonoBehaviour
{
    /// <summary>
    /// Base speed to move
    /// </summary>
    public float BaseMoveSpeed = 5f;
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
    /// Position to look at
    /// </summary>
    protected Vector3 lookPosition;
    /// <summary>
    /// Direction vector to look at
    /// </summary>
    protected Vector3 lookDirection;

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

    /// <summary>
    /// Handle input for movement
    /// </summary>
    protected virtual void HandleMovementInput()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.z = Input.GetAxisRaw("Vertical");
        moveDirection.Normalize();
    }
    /// <summary>
    /// Perform movment
    /// </summary>
    protected virtual void PerformMovement()
    {
        //body.MovePosition(body.position + moveDirection * CurrentMoveSpeed * Time.fixedDeltaTime);
        body.AddForce(moveDirection * CurrentMoveSpeed, ForceMode.Impulse);
    }
    /// <summary>
    /// Handle input to look
    /// </summary>
    protected virtual void HandleLookInput(){}

    /// <summary>
    /// Perform Look
    /// </summary>
    protected virtual void PerformLook()
    {
        lookDirection = lookPosition - body.position;
        var angle = Mathf.Atan2(lookDirection.z, lookDirection.x) * Mathf.Rad2Deg - LookOffSet;
        //body.rotation = Quaternion.Euler(0f,-angle,0f);
        body.MoveRotation(Quaternion.Euler(0f, -angle, 0f));
    }
}
