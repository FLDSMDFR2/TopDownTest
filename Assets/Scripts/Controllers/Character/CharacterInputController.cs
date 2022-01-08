using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterInputController : BaseInputController
{
    protected override void Update()
    {
        // movement & look
        base.Update();

        // check for fire input
        CheckForFire();
    }

    /// <summary>
    /// Override look handler to look at mouse location
    /// </summary>
    protected override void HandleLookInput()
    {
        // set location to look at
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane gPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (gPlane.Raycast(mouseRay, out rayLength))
        {
            lookPosition = mouseRay.GetPoint(rayLength);
        }
    }

    /// <summary>
    /// Check to see if fire button was pressed
    /// </summary>
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
    }

    #region Events
    /// <summary>
    /// Event to raise when firing 
    /// </summary>
    public event Action OnFirePrimary;
    public void FirePrimary()
    {
        if (OnFirePrimary != null)
        {
            OnFirePrimary();
        }
    }
    #endregion
}