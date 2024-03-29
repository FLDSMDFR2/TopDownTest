﻿using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class BaseCharacter : MonoBehaviour
{
    [Header("Base Character")]
    /// <summary>
    /// Character ID
    /// </summary>
    public int ID;
    /// <summary>
    /// Base health 
    /// </summary>
    public float BaseHealth;
    /// <summary>
    /// Current health
    /// </summary>
    public float CurentHealth;
    /// <summary>
    /// Pos to place weapon
    /// </summary>
    [SerializeField]
    protected Transform WeaponHoldPos;

    //********PROBABLY SHOULD REMOVE THIS FOR BETTER SOLUTION******
    /// <summary>
    /// Weapon to use
    /// </summary>
    [SerializeField]
    protected GameObject WeaponSlot;
    /// <summary>
    /// Shield to use
    /// </summary>
    [SerializeField]
    protected GameObject ShieldSlot;
    /// <summary>
    /// Shield to use
    /// </summary>
    [SerializeField]
    protected GameObject BatterySlot;
    //********PROBABLY SHOULD REMOVE THIS FOR BETTER SOLUTION******

    /// <summary>
    /// Active character weapon to use
    /// </summary>
    protected BaseWeapon activeWeapon;
    public BaseWeapon ActiveWeapon { get { return activeWeapon; } }
    /// <summary>
    /// Active character shield to use
    /// </summary>
    protected BaseShield Shield;
    /// <summary>
    /// Base Battery
    /// </summary>
    protected BaseBattery battery;
    public BaseBattery Battery { get { return battery; } }
    /// <summary>
    /// Field of view for this character
    /// </summary>
    [HideInInspector]
    public FieldOfView FOV;

    protected virtual void Awake()
    {
        FOV = GetComponent<FieldOfView>();

        InitalizeHealth();
        LoadInventoy();
    }

    protected virtual void Start()
    {
        // init things for the battery consume cost
        if (battery != null)
        {
            battery.Character = this;
            battery.SetConsumeAmountPerSec(ActiveWeapon.UpKeepCost());
            if (Shield != null)
            {
                battery.SetConsumeAmountPerSec(Shield.UpKeepCost());
            }
        }
    }

    /// <summary>
    /// Load character Invetory
    /// </summary>
    protected virtual void LoadInventoy()
    {
        //TODO: THIS SHOULD BE BETTER
        activeWeapon = WeaponSlot.GetComponentInChildren<BaseWeapon>(true);
        Shield = ShieldSlot.GetComponentInChildren<BaseShield>(true);
        if (BatterySlot != null) battery = BatterySlot.GetComponent<BaseBattery>();

        // set the weapon to active
        ActiveWeapon.SetWeapon(this, WeaponHoldPos);
    }

    /// <summary>
    /// Perform Health init
    /// </summary>
    public virtual void InitalizeHealth()
    {
        CurentHealth = MaxHealth();
    }

    /// <summary>
    /// Fire the active weapon
    /// </summary>
    protected virtual void FireWeapon()
    {
        ActiveWeapon.SetWeapon(this, WeaponHoldPos);
        ActiveWeapon.Fire();
    }

    /// <summary>
    /// Handle character taking damage
    /// </summary>
    /// <param name="damageTaken"></param>
    public virtual void TakeDamage(float damageTaken)
    {
        //if we have a shield
        if (Shield != null)
        {
            //check if we can deflect the damage
            if (BatterUseCheck(damageTaken) != BatteryPowerLevels.Critical)
                return;
        }

        CurentHealth = Mathf.Clamp(CurentHealth -= damageTaken, 0, MaxHealth());

        if (CurentHealth <= 0f)
            Dead();
    }

    /// <summary>
    ///  Check the battery power level
    /// </summary>
    /// <param name="cost">how much to use from battery</param>
    /// <returns>level of batter</returns>
    public virtual BatteryPowerLevels BatterUseCheck(float cost)
    {
        if (battery != null)
        {
            return battery.ConsumePower(cost);
        }

        // anything requesting should have a battery if not return low
        return BatteryPowerLevels.Low;
    }

    /// <summary>
    /// Return Max Health for Character BaseHealth + modifiers
    /// </summary>
    /// <returns></returns>
    protected virtual float MaxHealth()
    {
        // add modifiers to base health to get max
        return BaseHealth;
    }

    /// <summary>
    /// Handles Death
    /// </summary>
    protected virtual void Dead()
    {
        //override this
    }

    protected virtual void OnDestroy()
    {
        //clean up
    }
}
