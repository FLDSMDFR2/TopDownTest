using UnityEngine;

public class BaseWeapon : ModifiableItem
{
    #region Variables
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
    public FireModifierBase ShootingModifier { get { return shootingModifier; } }
    #endregion

    #region Item Init
    protected override void PerformAwake()
    {
        base.PerformAwake();

        CalculateDamage();
        CalculateRange();
        CalculateSpeed();
    }
    /// <summary>
    /// convert base data to our class specifc data
    /// </summary>
    protected override void CreateClassData()
    {
        ClassData = (BaseWeaponData)base.Data;
        if (ClassData == null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "BaseWeaponData Data set failed.");
        }
    }
    #endregion

    #region ModifiableItem
    public virtual void SetShootingModifier(WeaponModifier mod)
    {
        if (shootingModifier != null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.warning, "Multiple shooting modifiers added.");
            return;
        }
        shootingModifier = (FireModifierBase)mod;
    }
    #endregion

    #region Class Logic
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
    #endregion
}
