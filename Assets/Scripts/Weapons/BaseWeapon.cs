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
        var proj = Instantiate(Projectial, FirePos.position, FirePos.rotation);
        var baseProj = proj.GetComponent<BaseProjectile>();

        if (baseProj != null)
        {
            baseProj.SetCharacterId(_characterId);
        }

        Destroy(proj, Range);
    }
}
