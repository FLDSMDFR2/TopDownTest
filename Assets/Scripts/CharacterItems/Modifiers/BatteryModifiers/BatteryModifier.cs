using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryModifier : ItemModifier
{
    #region Variables
    [Header("Battery Modifier")]
    /// <summary>
    /// Data for this class
    /// </summary>
    [HideInInspector]
    public BatteryModifierData ClassData;
    #endregion

    #region Item Init
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
}
