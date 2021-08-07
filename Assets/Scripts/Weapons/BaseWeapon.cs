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

    protected virtual void FireProjectial()
    {
        float angleStep = (EndAngle - StartAngle) / ProjectileAmount;
        float angle =  FirePos.eulerAngles.y + StartAngle;

        for (int i = 0; i < ProjectileAmount; i++)
        {
            float dirX = FirePos.position.x + Mathf.Sin((angle * Mathf.PI) / 108f);
            float dirZ = FirePos.position.z + Mathf.Cos((angle * Mathf.PI) / 108f);

            Vector3 moveDir = new Vector3(dirX, FirePos.position.y, dirZ);
            Vector3 dir = (moveDir - FirePos.position).normalized;

            Quaternion rot = Quaternion.Euler(FirePos.rotation.eulerAngles.x, angle, FirePos.rotation.eulerAngles.z);

            var proj = Instantiate(Projectial, FirePos.position, rot);
            var baseProj = proj.GetComponent<BaseProjectile>();

            if (baseProj != null)
            {
                baseProj.SetCharacterId(_characterId);
                baseProj.SetDirection(dir);
            }

            Destroy(proj, Range);

            angle += angleStep;
        }
    }
}
