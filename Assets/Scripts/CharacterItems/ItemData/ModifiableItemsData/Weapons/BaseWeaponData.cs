using UnityEngine;

[CreateAssetMenu(fileName = "BaseWeaponData", menuName = "Items/Weapon/ModifiableItem/BaseWeapon")]
public class BaseWeaponData : ModifiableItemData
{
    [Header("Base Weapon")]
    /// <summary>
    /// Projectile prefab we will shoot (visual)
    /// </summary>
    public GameObject Projectial;
    /// <summary>
    /// Rate at which to shoot
    /// </summary>
    public float FireRate = 1f;
    /// <summary>
    /// Base Range of weapon
    /// </summary>
    public float BaseRange;
    /// <summary>
    /// Base speed of weapons projectile
    /// </summary>
    public float BaseSpeed;
    /// <summary>
    /// Base damge of weapon
    /// </summary>
    public float BaseDamage;
}
