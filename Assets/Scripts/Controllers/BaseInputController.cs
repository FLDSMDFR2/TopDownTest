using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInputController : MonoBehaviour
{
    public float DashSpeed = 10f;
    public float BaseSpeed = 5f;
    public float CurrentSpeed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public Vector3 Drag;

    public int MaxJumps = 2;
    public int jumpCount = 0;
    public Transform GroundChecker;

    protected Collider[] _myColliders;

    protected CharacterController _controller;
    protected Vector3 _velocity;
    protected bool _isGrounded = false;

    void Start()
    {
        _controller = GetComponent<CharacterController>();

        _myColliders = GetComponents<Collider>();

        CurrentSpeed = BaseSpeed;
    }
    protected virtual void HandleMovement()
    {

    }

    protected virtual void HandleLook()
    {

    }

    protected virtual void ApplyGravityAndForce()
    {
        // add gravity 
        _velocity.y = _velocity.y + (Gravity * Time.deltaTime);
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        _velocity.x /= 1 + Drag.x * Time.deltaTime;
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        _velocity.z /= 1 + Drag.z * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
    }

    #region OnCollision

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Landed(true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Landed(false);
        }
    }
    #endregion

    protected virtual void Landed(bool landed)
    {
        _isGrounded = landed;

        if (landed)
        {
            jumpCount = 0;
        }
    }
}
