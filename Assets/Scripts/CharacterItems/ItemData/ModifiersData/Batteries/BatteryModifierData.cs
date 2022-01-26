using UnityEngine;

[CreateAssetMenu(fileName = "BatteryModifierData", menuName = "Items/Batteries/Modifiers/BaseBatteryModifier")]
public class BatteryModifierData : WeaponModifierData
{
    [Header("Battery Modifier")]
    /// <summary>
    /// Power +/- to batter
    /// </summary>
    public float Power;
}
