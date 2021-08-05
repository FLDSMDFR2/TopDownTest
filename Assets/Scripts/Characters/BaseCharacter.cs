using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    [Header("Base Character")]
    public int CharacterID;

    public float MaxHealth;
    public float CurentHealth;

    public Transform HoldPos;

    public GameObject PrimaryWeaponSlot;
    public GameObject SecondaryWeaponSlot;
    public GameObject ShieldSlot;

    protected BaseWeapon PrimaryWeapon;
    protected BaseWeapon SecondaryWeapon;
    protected BaseShield Shield;

    protected Collider[] _myColliders;

    protected virtual void Awake()
    {
        InitalizeHealth();
         _myColliders = GetComponents<Collider>();
        LoadInventoy();
    }

    protected void LoadInventoy()
    {
        PrimaryWeapon = PrimaryWeaponSlot.GetComponentInChildren<BaseWeapon>(true);
        SecondaryWeapon = SecondaryWeaponSlot.GetComponentInChildren<BaseWeapon>(true);
        Shield = ShieldSlot.GetComponentInChildren<BaseShield>(true);
    }

    public virtual void InitalizeHealth()
    {
        CurentHealth = MaxHealth;
    }

    protected virtual void FirePrimaryWeapon()
    {
        PrimaryWeapon.SetWeapon(CharacterID, HoldPos);
        PrimaryWeapon.Fire();
    }

    public virtual void TakeDamage(float damageTaken)
    {

        if (Shield != null)
        {
            damageTaken = Shield.TakeDamage(damageTaken);
        }

        CurentHealth = Mathf.Clamp(CurentHealth -= damageTaken, 0, MaxHealth);

        if (CurentHealth <= 0f)
            Dead();

    }

    protected virtual void Dead()
    {

    }

}
