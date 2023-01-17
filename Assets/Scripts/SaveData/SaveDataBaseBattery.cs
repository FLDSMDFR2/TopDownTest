using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum BatteryPowerLevels
{
    Critical,
    Low,
    Medium,
    High
}

[Serializable]
public class BatteryLevel
{
    public BatteryPowerLevels Level;
    public float PowerPercent;
    public Color DisplayColor;
}

[Serializable]
public class SaveDataBaseBattery : BaseSaveableObj
{
    /// <summary>
    /// Max power in battery
    /// </summary>
    public float BaseMaxPower;
    /// <summary>
    /// the rate at which we consume power
    /// </summary>
    public float Rate = 1;
    /// <summary>
    /// List of power levels
    /// </summary>
    public List<BatteryLevel> PowerLevels;
}

