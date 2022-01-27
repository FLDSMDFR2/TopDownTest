using UnityEngine;

public class LaserSightModifier : WeaponModifier
{
    #region Variables
    [Header("Laser Sight Modifier")]
    /// <summary>
    /// line render for the laser sight
    /// </summary>
    protected LineRenderer lineRenderer;
    /// <summary>
    /// Position to start laser from
    /// </summary>
    protected Transform startPos;
    /// <summary>
    /// distance laser will go out
    /// </summary>
    protected float distance;
    #endregion

    #region WeaponModifier
    public override void InitWeaponModifier(BaseWeapon weapon)
    {
        base.InitWeaponModifier(weapon);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .01f;

        startPos = weapon.FirePos;
    }
    #endregion

    #region Class Logic
    protected virtual void Update()
    {
        if (startPos != null)
        {
            lineRenderer.SetPosition(0, startPos.position);
            lineRenderer.SetPosition(1, startPos.position + (startPos.forward * weapon.Range));
        }
    }
    #endregion
}
