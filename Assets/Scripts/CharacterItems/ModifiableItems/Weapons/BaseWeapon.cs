using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : ModifiableItem
{
    [Header("Base Weapon")]
    /// <summary>
    /// Data for this class
    /// </summary>
    [HideInInspector]
    public BaseWeaponData ClassData;
    /// <summary>
    /// Location to shoot from
    /// </summary>
    public Transform FirePos;
    /// <summary>
    /// Range of weapon
    /// </summary>
    protected float range;
    public float Range
    {
        get { return range; }
        set { range = value; }
    }
    /// <summary>
    /// Speed of weapons projectile
    /// </summary>
    protected float speed;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    /// <summary>
    /// Damge of weapon
    /// </summary>
    protected float damage;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    /// <summary>
    /// Character assigned to this weapon
    /// </summary>
    protected BaseCharacter character;
    public BaseCharacter Character { get { return character; } }
    /// <summary>
    /// if this weapon is active
    /// </summary>
    protected bool isActiveWeapon = false;

    protected FireModifierBase shootingModifier;

    protected override void PerformAwake()
    {
        base.PerformAwake();

        CalculateDamage();
        CalculateRange();
        CalculateSpeed();
    }

    protected override void CreateClassData()
    {
        ClassData = (BaseWeaponData)base.Data;
        if (ClassData == null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "BaseWeaponData Data set failed.");
        }
    }

    /// <summary>
    /// Set weapon active for character
    /// </summary>
    /// <param name="characterID"></param>
    /// <param name="holdPos"></param>
    public virtual void SetWeapon(BaseCharacter character, Transform holdPos)
    {
        if (isActiveWeapon)
            return;

        this.character = character;
        transform.position = holdPos.position;
        transform.rotation = holdPos.rotation;

        gameObject.SetActive(true);

        isActiveWeapon = true;
    }

    /// <summary>
    /// Clear weapon active for character
    /// </summary>
    public virtual void SetClearWeapon()
    {
        if (!isActiveWeapon)
            return;

        gameObject.SetActive(false);

        isActiveWeapon = false;
    }

    /// <summary>
    /// fire weapon
    /// </summary>
    public virtual void Fire()
    {
        // only fire if this weapon is active
        if (!isActiveWeapon)
            return;

        // fire
        if (shootingModifier != null) shootingModifier.Fire();
    }

    /// <summary>
    /// Damage of weapon Base damage + modifiers
    /// </summary>
    /// <returns></returns>
    protected virtual void CalculateDamage()
    {
        Damage += ClassData.BaseDamage;
    }

    /// <summary>
    /// Speed of weapon projectiles Base speeed + modifiers
    /// </summary>
    /// <returns></returns>
    protected virtual void CalculateSpeed()
    {
        Speed += ClassData.BaseSpeed;
    }
    /// <summary>
    /// Range of weapon Base range + modifiers
    /// </summary>
    /// <returns></returns>
    protected virtual void CalculateRange()
    {
        Range += ClassData.BaseRange;
    }

    #region Modifiers
    protected virtual void SetShootingModifier(WeaponModifier mod)
    {
        if (shootingModifier != null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.warning, "Multiple shooting modifiers added.");
            return;
        }
        shootingModifier = (FireModifierBase)mod;
    }

    protected override void SetModifiers()
    {
        base.SetModifiers();

        var mods = GetModifierByType(typeof(WeaponModifier));
        foreach(var mod in mods)
        {
            var weaponMode = mod as WeaponModifier;
            if (weaponMode == null) continue;

            if (weaponMode is FireModifierBase)
            {
                SetShootingModifier(weaponMode);
            }
            weaponMode.InitWeaponModifier(this);
        }
    }
    #endregion
}
