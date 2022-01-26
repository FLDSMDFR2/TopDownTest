using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BaseBatteryData", menuName = "Items/Batteries/ModifiableItem/BaseBattery")]
public class BaseBatteryData : ModifiableItemData
{
    [Header("Base Battery")]
    /// <summary>
    /// Max power in battery
    /// </summary>
    public float BaseMaxPower;
    /// <summary>
    /// the rate at which we consume power
    /// </summary>
    public float Rate = 1;
}
