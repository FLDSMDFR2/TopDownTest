using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryModifier : ItemModifier
{
    [Header("Battery Modifier")]
    /// <summary>
    /// Data for this class
    /// </summary>
    [HideInInspector]
    public BatteryModifierData ClassData;

    protected override void CreateClassData()
    {
        ClassData = (BatteryModifierData)base.Data;
        if (ClassData == null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "BatteryModifierData Data set failed.");
        }
    }
}
