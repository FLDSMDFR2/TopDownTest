using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireModifierBase : WeaponModifier
{
    #region Variables
    [Header("Fire Modifier")]
    /// <summary>
    /// Data for this class
    /// </summary>
    [HideInInspector]
    public FireModifierData ClassData;
    /// <summary>
    /// If we are currently firing
    /// </summary>
    protected bool isFiring = false;
    /// <summary>
    /// Last time we fired
    /// </summary>
    protected float lastFire = 0f;
    /// <summary>
    /// position to fire from
    /// </summary>
    protected Transform firePos;
    /// <summary>
    /// Projectial to fire
    /// </summary>
    protected GameObject projectileObject;
    /// <summary>
    /// Data class for projectial
    /// </summary>
    protected BaseProjectile projectile;
    #endregion

    #region Item Init
    /// <summary>
    /// convert base data to our class specifc data
    /// </summary>
    protected override void CreateClassData()
    {
        ClassData = (FireModifierData)base.Data;
        if (ClassData == null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "FireModifierData Data set failed.");
        }
    }
    #endregion

    #region Item Overrides
    /// <summary>
    /// Cost to Use
    /// </summary>
    /// <returns></returns>
    public override float UseCost()
    {
        return Data.UseCost * (ClassData.ProjectileAmount * ClassData.BurstAmount);
    }
    #endregion

    #region WeaponModifier
    /// <summary>
    /// Assign modifier details to the item we will modify
    /// </summary>
    protected override void AssignModifierDetails()
    {
        base.AssignModifierDetails();

        weapon.SetShootingModifier(this);
        firePos = weapon.FirePos;
        projectileObject = weapon.ClassData.Projectial;
        projectile = projectileObject.GetComponent<BaseProjectile>();
    }
    #endregion

    #region Class Logic
    /// <summary>
    /// Perform Fire funcunality for this modifer
    /// </summary>
    public virtual void Fire()
    {
        // if last fire is 0 then fire this is the first shot
        // or fire when rate allows && we are currently not firing
        if (!isFiring && (lastFire <= 0f || Time.time >= lastFire + weapon.ClassData.FireRate))
        {
            if (weapon.Character.BatterUseCheck(weapon.UseCost()))
            {
                isFiring = true;
                StartCoroutine(FireProjectial());
            }
        }
    }

    /// <summary>
    /// Create and Fire funcunality for this modifer
    /// </summary>
    protected virtual IEnumerator FireProjectial()
    {
        if (ClassData.BurstAmount <= 0) ClassData.BurstAmount = 1;
        var burstCount = 0;
        // perform burst fire if needed
        while (burstCount < ClassData.BurstAmount)
        {
            // fire projectile pattern
            var pa = ClassData.ProjectileAmount - 1;
            if (pa <= 0) pa = 1;

            float angleStep = (ClassData.EndAngle - ClassData.StartAngle) / pa;
            float angle = firePos.eulerAngles.y + ClassData.StartAngle;

            for (int i = 0; i < ClassData.ProjectileAmount; i++)
            {
                Vector3 dir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0F, Mathf.Cos(Mathf.Deg2Rad * angle));
                Quaternion rot = Quaternion.Euler(firePos.rotation.eulerAngles.x + 90, angle, firePos.rotation.eulerAngles.z);

                var proj = GOPoolManager.GetObject(projectile.GetPoolId(), projectileObject, firePos.position, rot);
                var baseProj = proj.GetComponentInChildren<BaseProjectile>();

                if (baseProj != null)
                {
                    baseProj.InitProjectile(weapon.Character.ID, dir, weapon.Range, weapon.Speed, weapon.Damage);
                    baseProj.Fire();
                }

                angle += angleStep;
            }
            burstCount++;
            if (ClassData.BurstAmount > 1) yield return new WaitForSeconds(ClassData.BurstRate);
        }

        // firing is complete
        isFiring = false;
        lastFire = Time.time;
    }
    #endregion
}
