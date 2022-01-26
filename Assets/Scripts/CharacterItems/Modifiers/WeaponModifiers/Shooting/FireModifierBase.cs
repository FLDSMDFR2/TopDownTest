using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireModifierBase : WeaponModifier
{
    [Header("Fire Modifier Base")]
    /// <summary>
    /// Rate at which to shoot
    /// </summary>
    public float FireRate = 1f;
    /// <summary>
    /// Amount of projectiles to shoot
    /// </summary>
    public int ProjectileAmount = 1;
    /// <summary>
    /// Start angle of shoot spread
    /// </summary>
    public float StartAngle = 180;
    /// <summary>
    /// End angle of shoot spread
    /// </summary>
    public float EndAngle = 180;

    [Header("Burst")]
    /// <summary>
    /// Number of burst to perform
    /// </summary>
    public int BurstAmount = 1;
    /// <summary>
    /// time between each burst shot
    /// </summary>
    public float BurstRate = 0.5f;
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

    public override void InitWeaponModifier(BaseWeapon weapon)
    {
        base.InitWeaponModifier(weapon);

        firePos = weapon.FirePos;
        projectileObject = weapon.Projectial;
        projectile = projectileObject.GetComponent<BaseProjectile>();
    }

    /// <summary>
    /// Cost to Use
    /// </summary>
    /// <returns></returns>
    public override float UseCost()
    {
        return useCost * (ProjectileAmount * BurstAmount);
    }

    /// <summary>
    /// Perform Fire funcunality for this modifer
    /// </summary>
    public virtual void Fire()
    {
        // if last fire is 0 then fire this is the first shot
        // or fire when rate allows && we are currently not firing
        if (!isFiring && (lastFire <= 0f || Time.time >= lastFire + FireRate))
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
        if (BurstAmount <= 0) BurstAmount = 1;
        var burstCount = 0;
        // perform burst fire if needed
        while (burstCount < BurstAmount)
        {
            // fire projectile pattern
            var pa = ProjectileAmount - 1;
            if (pa <= 0) pa = 1;

            float angleStep = (EndAngle - StartAngle) / pa;
            float angle = firePos.eulerAngles.y + StartAngle;

            for (int i = 0; i < ProjectileAmount; i++)
            {
                Vector3 dir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0F, Mathf.Cos(Mathf.Deg2Rad * angle));
                Quaternion rot = Quaternion.Euler(firePos.rotation.eulerAngles.x + 90, angle, firePos.rotation.eulerAngles.z);

                var proj = GOPoolManager.GetObject(projectile.GetPoolId(), projectileObject, firePos.position, rot);
                var baseProj = proj.GetComponentInChildren<BaseProjectile>();

                if (baseProj != null)
                {
                    baseProj.InitProjectile(weapon.Character.ID, dir, weapon.Range, weapon.Speed,weapon.Damage);
                    baseProj.Fire();
                }

                angle += angleStep;
            }
            burstCount++;
            if (BurstAmount > 1) yield return new WaitForSeconds(BurstRate);
        }

        // firing is complete
        isFiring = false;
        lastFire = Time.time;
    }
}
