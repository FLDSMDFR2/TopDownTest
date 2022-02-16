using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerItemObject : RoomItemInteractableObject
{
    public float ChargerPowerRate;
    public float ChargerPowerAmount;

    protected bool isCharging;

    /// <summary>
    /// Perform Intercation logic
    /// </summary>
    /// <param name="character"></param>
    public override void PerformInteract(BaseCharacter character)
    {
        base.PerformInteract(character);

        if (character != null && character.Battery != null)
        {
            StartCoroutine(ChargeBattery(character.Battery));
        }
    }

    /// <summary>
    /// Perform Battery Charging
    /// </summary>
    /// <param name="battery"></param>
    /// <returns></returns>
    protected virtual IEnumerator ChargeBattery(BaseBattery battery)
    {
        if (isCharging) yield break;
        isCharging = true;

        while (!IsOutOfInteractRange())
        {
            yield return new WaitForSeconds(ChargerPowerRate);

            battery.IncreasePower(ChargerPowerAmount);

            yield return null;
        }

        isCharging = false;
    }
}
