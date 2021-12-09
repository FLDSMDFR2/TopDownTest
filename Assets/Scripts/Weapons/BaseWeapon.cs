using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    public Transform FirePos;
    //public Transform HoldPos;

    public GameObject Projectial;
    public float Range;

    public float FireRate = 1f;

    public int ProjectileAmount = 1;

    public float StartAngle = 180;
    public float EndAngle = 180;
    private Vector3 projectialDirection = Vector3.forward;

    protected float lastFire = 0f;

    protected bool isActiveWeapon = false;

    private int _characterId;

    public virtual void SetWeapon(int CharacterID, Transform HoldPos)
    {
        if (isActiveWeapon)
            return;

        _characterId = CharacterID;
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
        if (!isActiveWeapon)
            return;
     
        if (lastFire <= 0f || Time.time >= lastFire + FireRate)
        {
            FireProjectial();
            lastFire = Time.time;
        }
    }
    protected virtual System.Type GetProjectile()
    {
        return typeof(BaseProjectile);
    }

    protected virtual void FireProjectial()
    {
        var pa = ProjectileAmount - 1;
        if (pa <= 0) pa = 1;

        float angleStep = (EndAngle - StartAngle) / pa;
        float angle =  FirePos.eulerAngles.y + StartAngle;

        for (int i = 0; i < ProjectileAmount; i++)
        {
            Vector3 dir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0F, Mathf.Cos(Mathf.Deg2Rad * angle));
            Quaternion rot = Quaternion.Euler(FirePos.rotation.eulerAngles.x, angle, FirePos.rotation.eulerAngles.z);

            var proj = GOPoolManager.GetObject(GetProjectile(), Projectial, FirePos.position, rot);
            var baseProj = proj.GetComponent<BaseProjectile>();

            if (baseProj != null)
            {
                baseProj.SetCharacterId(_characterId);
                baseProj.SetDirection(dir);
                baseProj.SetRange(Range);
            }

            angle += angleStep;
        }
    }
}
