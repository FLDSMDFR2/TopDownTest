using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    [Header("Base Weapon")]
    public LineRenderer lineRenderer;

    public Transform FirePos;
    public GameObject Projectial;
    public float Range;

    [Header("Base Fire Controls")]
    public float FireRate = 1f;

    public int ProjectileAmount = 1;

    public int BurstAmount = 1;
    public float BurstRate = 0.5f;

    public float StartAngle = 180;
    public float EndAngle = 180;

    protected bool isFiring = false;
    protected float lastFire = 0f;
    protected bool isActiveWeapon = false;
    protected int characterId;

    protected virtual void Update()
    {
        lineRenderer.SetPosition(0, FirePos.position);
        lineRenderer.SetPosition(1, FirePos.position + (FirePos.forward * Range));
    }

    public virtual void SetWeapon(int CharacterID, Transform HoldPos)
    {
        if (isActiveWeapon)
            return;

        characterId = CharacterID;
        transform.position = HoldPos.position;
        transform.rotation = HoldPos.rotation;
        gameObject.SetActive(true);

        isActiveWeapon = true;
    }

    public virtual void SetClearWeapon()
    {
        if (!isActiveWeapon)
            return;

        gameObject.SetActive(false);

        isActiveWeapon = false;
    }

    public virtual void Fire()
    {
        // only fire if this weapon is active
        if (!isActiveWeapon)
            return;

        // if last fire is 0 then fire this is the first shot
        // or fire when rate allows && we are currently not firing
        if (!isFiring && (lastFire <= 0f || Time.time >= lastFire + FireRate))
        {
            isFiring = true;
            StartCoroutine(FireProjectial());
        }
    }

    protected virtual System.Type GetProjectileType()
    {
        return typeof(BaseProjectile);
    }

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
            float angle = FirePos.eulerAngles.y + StartAngle;

            for (int i = 0; i < ProjectileAmount; i++)
            {
                Vector3 dir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0F, Mathf.Cos(Mathf.Deg2Rad * angle));
                Quaternion rot = Quaternion.Euler(FirePos.rotation.eulerAngles.x, angle, FirePos.rotation.eulerAngles.z);

                var proj = GOPoolManager.GetObject(GetProjectileType(), Projectial, FirePos.position, rot);
                var baseProj = proj.GetComponentInChildren<BaseProjectile>();

                if (baseProj != null)
                {
                    baseProj.Reset();
                    baseProj.SetCharacterId(characterId);
                    baseProj.SetDirection(dir);
                    baseProj.SetRange(Range);

                    baseProj.Fire();
                }

                angle += angleStep;
            }
            burstCount++;
            yield return new WaitForSeconds(BurstRate);
        }

        // firing is complete
        isFiring = false;
        lastFire = Time.time;
    }
}
