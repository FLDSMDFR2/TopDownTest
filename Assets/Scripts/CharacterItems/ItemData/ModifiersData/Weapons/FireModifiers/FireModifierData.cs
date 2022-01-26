using UnityEngine;

[CreateAssetMenu(fileName = "FireModifierData", menuName = "Items/Weapon/Modifiers/FireModifier")]
public class FireModifierData : WeaponModifierData
{
    [Header("Fire Modifier Base")]
    /// <summary>
    /// Amount of projectiles to shoot
    /// </summary>
    public int ProjectileAmount = 1;
    /// <summary>
    /// Start angle of shoot spread
    /// </summary>
    public float StartAngle = 180;
    /// <summary>
    /// End angle of shoot spread
    /// </summary>
    public float EndAngle = 180;

    [Header("Burst")]
    /// <summary>
    /// Number of burst to perform
    /// </summary>
    public int BurstAmount = 1;
    /// <summary>
    /// time between each burst shot
    /// </summary>
    public float BurstRate = 0.5f;
}
