using UnityEngine;

[RequireComponent(typeof(BaseBattery))]
public class BatteryModifier : ItemModifier
{
    #region Variables
    /// <summary>
    /// battery this modifer is used for
    /// </summary>
    protected BaseBattery battery;
    #endregion

    #region Class Init
    protected override void PerformAwake()
    {
        base.PerformAwake();

        battery = this.GetComponent<BaseBattery>();
    }
    #endregion

    #region Class Logic
    /// <summary>
    /// Assign modifier details to the item we will modify
    /// </summary>
    protected override void AssignModifierDetails()
    {
        battery.SetModifierUseCosts(Data.UseCost);
        battery.SetModifierUpKeepCosts(Data.UpKeepCost);
    }
    #endregion
}
