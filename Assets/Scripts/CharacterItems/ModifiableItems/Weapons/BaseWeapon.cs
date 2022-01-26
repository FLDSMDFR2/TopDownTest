using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : ModifiableItem
{
    [Header("Base Weapon")]
    /// <summary>
    /// Location to shoot from
    /// </summary>
    public Transform FirePos;
    /// <summary>
    /// Projectile prefab we will shoot (visual)
    /// </summary>
    public GameObject Projectial;
    /// <summary>
    /// Base Range of weapon
    /// </summary>
    [SerializeField]
    protected float BaseRange;
    protected float range;
    public float Range { get { return range; } }
    /// <summary>
    /// Base speed of weapons projectile
    /// </summary>
    [SerializeField]
    protected float BaseSpeed;
    protected float speed;
    public float Speed { get { return speed; } }
    /// <summary>
    /// Base damge of weapon
    /// </summary>
    [SerializeField]
    protected float BaseDamage;
    protected float damage;
    public float Damage { get { return damage; } }
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

    protected override void Awake()
    {
        base.Awake();

        CalculateDamage();
        CalculateRange();
        CalculateSpeed();
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
        damage += BaseDamage;
    }

    /// <summary>
    /// Speed of weapon projectiles Base speeed + modifiers
    /// </summary>
    /// <returns></returns>
    protected virtual void CalculateSpeed()
    {
        speed += BaseSpeed;
    }
    /// <summary>
    /// Range of weapon Base range + modifiers
    /// </summary>
    /// <returns></returns>
    protected virtual void CalculateRange()
    {
        range += BaseRange;
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
