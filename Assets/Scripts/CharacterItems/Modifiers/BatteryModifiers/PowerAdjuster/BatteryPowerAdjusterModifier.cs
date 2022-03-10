using UnityEngine;

public class BatteryPowerAdjusterModifier : BatteryModifier
{

    #region Variables
    [Header("Battery Power Adjuster Modifier")]
    /// <summary>
    /// Data for this class
    /// </summary>
    [HideInInspector]
    public BatteryModifierData ClassData;
    #endregion

    #region Class Init
    /// <summary>
    /// convert base data to our class specifc data
    /// </summary>
    protected override void CreateClassData()
    {
        ClassData = (BatteryModifierData)base.Data;
        if (ClassData == null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "BatteryModifierData Data set failed.");
        }
    }
    #endregion

    #region Modifier
    /// <summary>
    /// Assign modifier details to the item we will modify
    /// </summary>
    protected override void AssignModifierDetails()
    {
        base.AssignModifierDetails();

        battery.UpdateMaxPower(ClassData.Power);
    }
    #endregion

}
