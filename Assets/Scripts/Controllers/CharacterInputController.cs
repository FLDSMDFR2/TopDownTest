using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterInputController : BaseInputController
{
    void Update()
    {

        if (CheckForStopped())
        {
            CurrentSpeed = BaseSpeed;
        }

        if (CheckForDash())
        {
            CurrentSpeed = DashSpeed;
        }

        HandleMovement();

        HandleLook();

        HandleJump();

        CheckForDropThrough();

        ApplyGravityAndForce();

        CheckForFire();
    }

    protected override void HandleMovement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0f);
        move = move.normalized * Time.deltaTime * CurrentSpeed;

        _controller.Move(move);

        if (move != Vector3.zero)
            transform.forward = move;
    }


    protected virtual void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && (jumpCount < MaxJumps))
        {
            _velocity.y = JumpHeight;
            jumpCount++;
        }
    }

    protected virtual bool CheckForStopped()
    {
        return Input.GetAxis("Horizontal") == 0f;
    }

    protected virtual void CheckForFire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FirePrimary();
        }
        else if(Input.GetMouseButton(0))
        {
            FirePrimary();
        }
        else if (Input.GetMouseButtonDown(2))
        {
            FireSecondary();
        }
        else if (Input.GetMouseButton(2))
        {
            FireSecondary();
        }
    }


    #region DropThrough
    private float DropThroughCooler = 0.5f; // Half a second before reset
    private int DropThroughCount = 0;
    private void CheckForDropThrough()
    {
        if (Input.GetButtonDown("Down"))
        {

            if (DropThroughCooler > 0 && DropThroughCount == 1/*Number of Taps you want Minus One*/)
            {
                DisableCollisionForCurrentPlatform();
            }
            else
            {
                DropThroughCooler = 0.5f;
                DropThroughCount += 1;
            }
        }

        if (DropThroughCooler > 0)
        {

            DropThroughCooler -= 1 * Time.deltaTime;

        }
        else
        {
            DropThroughCount = 0;
        }
    }

    private void DisableCollisionForCurrentPlatform()
    {

    }
    #endregion

    #region CheckForDash

    protected virtual bool CheckForDash()
    {
        if (CheckForLeftDash())
        {
            return true;
        }

        if (CheckForRightDash())
        {
            return true;
        }

        return false;
    }

    private float DashLeftCooler = 0.5f; // Half a second before reset
    private int DashLeftCount = 0;
    private bool CheckForLeftDash()
    {

        bool retVal = false;
        if (Input.GetButtonDown("Left"))
        {
           
            if (DashLeftCooler > 0 && DashLeftCount == 1/*Number of Taps you want Minus One*/)
            {
                retVal = true;
            }
            else
            {
                DashLeftCooler = 0.5f;
                DashLeftCount += 1;
            }
        }

        if (DashLeftCooler > 0)
        {

            DashLeftCooler -= 1 * Time.deltaTime;

        }
        else
        {
            DashLeftCount = 0;
        }

        return retVal;
    }

    private float DashRightCooler = 0.5f; // Half a second before reset
    private int DashRightCount = 0;
    private bool CheckForRightDash()
    {
        bool retVal = false;
        if (Input.GetButtonDown("Right"))
        {

            if (DashRightCooler > 0 && DashRightCount == 1/*Number of Taps you want Minus One*/)
            {
                retVal = true;
            }
            else
            {
                DashRightCooler = 0.5f;
                DashRightCount += 1;
            }
        }

        if (DashRightCooler > 0)
        {

            DashRightCooler -= 1 * Time.deltaTime;

        }
        else
        {
            DashRightCount = 0;
        }

        return retVal;
    }
    #endregion

    #region Events
    //public delegate void FireDelegate(bool ContinuousFire);
    public event Action OnFirePrimary;
    public void FirePrimary()
    {
        if (OnFirePrimary != null)
        {
            OnFirePrimary();
        }
    }

    public event Action OnFireSecondary;
    public void FireSecondary()
    {
        if (OnFireSecondary != null)
        {
            OnFireSecondary();
        }
    }
    #endregion

}