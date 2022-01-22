using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    [Header("Base Character")]
    /// <summary>
    /// Character ID
    /// </summary>
    public int CharacterID;
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
    public Transform WeaponHoldPos;

    //********PROBABLY SHOULD REMOVE THIS FOR BETTER SOLUTION******
    /// <summary>
    /// Weapon to use
    /// </summary>
    public GameObject WeaponSlot;
    /// <summary>
    /// Shield to use
    /// </summary>
    public GameObject ShieldSlot;
    //********PROBABLY SHOULD REMOVE THIS FOR BETTER SOLUTION******

    /// <summary>
    /// Active character weapon to use
    /// </summary>
    protected BaseWeapon ActiveWeapon;
    /// <summary>
    /// Active character shield to use
    /// </summary>
    protected BaseShield Shield;

    protected virtual void Awake()
    {
        InitalizeHealth();
        LoadInventoy();
    }

    /// <summary>
    /// Load character Invetory
    /// </summary>
    protected virtual void LoadInventoy()
    {
        //TODO: THIS SHOULD BE BETTER
        ActiveWeapon = WeaponSlot.GetComponentInChildren<BaseWeapon>(true);
        Shield = ShieldSlot.GetComponentInChildren<BaseShield>(true);
        // set the weapon to active
        ActiveWeapon.SetWeapon(CharacterID, WeaponHoldPos);
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
        ActiveWeapon.SetWeapon(CharacterID, WeaponHoldPos);
        ActiveWeapon.Fire();
    }

    /// <summary>
    /// Handle character taking damage
    /// </summary>
    /// <param name="damageTaken"></param>
    public virtual void TakeDamage(float damageTaken)
    {

        if (Shield != null)
        {
            damageTaken = Shield.TakeDamage(damageTaken);
        }

        CurentHealth = Mathf.Clamp(CurentHealth -= damageTaken, 0, MaxHealth());

        if (CurentHealth <= 0f)
            Dead();
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
}
