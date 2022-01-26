using UnityEngine;

public class LaserSightModifier : WeaponModifier
{
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

    public override void InitWeaponModifier(BaseWeapon weapon)
    {
        base.InitWeaponModifier(weapon);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .01f;

        startPos = weapon.FirePos;
    }

    protected virtual void Update()
    {
        if (startPos != null)
        {
            lineRenderer.SetPosition(0, startPos.position);
            lineRenderer.SetPosition(1, startPos.position + (startPos.forward * weapon.Range));
        }
    }
}
