using UnityEngine;

[CreateAssetMenu(fileName = "BaseBatteryData", menuName = "Items/Batteries/ModifiableItem/BaseBattery")]
public class BaseBatteryData : ModifiableItemData
{
    [Header("Base Battery")]
    public SaveDataBaseBattery Data;
}

