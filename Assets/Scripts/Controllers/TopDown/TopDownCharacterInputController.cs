using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterInputController : CharacterInputController
{

    public float dis = 1f;

    protected override void HandleMovement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        move = move.normalized * Time.deltaTime * CurrentSpeed;

        _controller.Move(move);

        if (move != Vector3.zero)
            transform.forward = move;
    }

    protected override void HandleLook()
    {

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane gPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (gPlane.Raycast(mouseRay, out rayLength))
        {
            Vector3 pointToLook = mouseRay.GetPoint(rayLength);          
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y,pointToLook.z));
        }

    }

    protected override bool CheckForStopped()
    {
        return Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f;
    }

    protected override void HandleJump()
    {
        //no jump for now
    }



    #region CheckForDash

    protected override bool CheckForDash()
    {
        if (base.CheckForDash())
            return true;

        if (CheckForForwardDash())
            return true;

        if (CheckForBackDash())
            return true;


        return false;
    }

    private float DashForwardCooler = 0.5f; // Half a second before reset
    private int DashForwardCount = 0;
    private bool CheckForForwardDash()
    {

        bool retVal = false;
        if (Input.GetButtonDown("Forward"))
        {

            if (DashForwardCooler > 0 && DashForwardCount == 1/*Number of Taps you want Minus One*/)
            {
                retVal = true;
            }
            else
            {
                DashForwardCooler = 0.5f;
                DashForwardCount += 1;
            }
        }

        if (DashForwardCooler > 0)
        {

            DashForwardCooler -= 1 * Time.deltaTime;

        }
        else
        {
            DashForwardCount = 0;
        }

        return retVal;
    }

    private float DashBackCooler = 0.5f; // Half a second before reset
    private int DashBackCount = 0;
    private bool CheckForBackDash()
    {
        bool retVal = false;
        if (Input.GetButtonDown("Down"))
        {

            if (DashBackCooler > 0 && DashBackCount == 1/*Number of Taps you want Minus One*/)
            {
                retVal = true;
            }
            else
            {
                DashBackCooler = 0.5f;
                DashBackCount += 1;
            }
        }

        if (DashBackCooler > 0)
        {

            DashBackCooler -= 1 * Time.deltaTime;

        }
        else
        {
            DashBackCount = 0;
        }

        return retVal;
    }
    #endregion
}

