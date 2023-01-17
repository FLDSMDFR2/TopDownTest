using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBattery : ModifiableItem
{
    #region Variables
    [Header("Base Battery")]
    /// <summary>
    /// Data for this class
    /// </summary>
    [SerializeField]
    protected SaveDataBaseBattery ClassData;
    /// <summary>
    /// Max power of batter with all modifiers applied
    /// </summary>
    protected float maxPower;
    /// <summary>
    /// Current power of battery
    /// </summary>
    protected float power = -1;
    protected float currentPower
    {
        set
        {
            power = value;
            CheckPowerLevel();
            if (character != null)
                UIEvents.RaisePowerUpdateEvent(character.ID, PowerLevel, power, maxPower);
        }
        get { return power; }
    }
    /// <summary>
    /// The Amount we will consume from the batter per second without doing anything
    /// This should be a comination of all general power consumtion ie (system power, shields, sensers...)
    /// </summary>
    protected float consumeAmountPerRate;
    /// <summary>
    /// If the battery system is running
    /// </summary>
    protected bool IsRunning;
    /// <summary>
    /// Character this battery is assigned to
    /// </summary>
    protected BaseCharacter character;
    public BaseCharacter Character { set { character = value; } }
    /// <summary>
    /// power level of battery
    /// </summary>
    protected BatteryLevel PowerLevel;
    #endregion

    #region Item Init
    /// <summary>
    /// convert base data to our class specifc data
    /// </summary>
    protected override void CreateClassData()
    {
        //load save data
        if (false)
        {
            //TODO: load in data
        }
        else
        {
            // get default data
            var configData = (BaseBatteryData)base.Data;
            ClassData = configData.Data;
        }

        if (ClassData == null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "BaseBatteryData Data set failed.");
        }
    }

    /// <summary>
    /// Init this class details
    /// </summary>
    protected override void InitClassDetails()
    {
        base.InitClassDetails();

        maxPower = ClassData.BaseMaxPower;
    }
    #endregion

    #region ModifiableItem
    /// <summary>
    /// Update Max Power
    /// </summary>
    public virtual void UpdateMaxPower(float power)
    {
        maxPower += power;
    }
    #endregion

    #region Class Logic
    /// <summary>
    /// When we power on battery we need to set a rate at which we will constume the battery power while doing nothing
    /// </summary>
    /// <param name="rate"></param>
    public virtual void SetConsumeAmountPerSec(float amount)
    {
        // stop the coroutine if its running as we might be resetting consume amount
        StopAllCoroutines();

        //set amount to consume
        consumeAmountPerRate += amount;

        //if we are init the power then set to max otherwise we are just updating the consume rate
        if (currentPower == -1) currentPower = maxPower;
        // set running if we have power
        if (currentPower > 0) IsRunning = true;

        //start to consume power
        StartCoroutine(ConsumeBattery());
    }

    /// <summary>
    /// Consume requested amount of battery
    /// </summary>
    /// <param name="amount">amount to consume</param>
    /// <returns>Return power level to requester</returns> 
    public virtual BatteryPowerLevels ConsumePower(float amount)
    {
        // constume the power and return the level of the battery
        currentPower = Mathf.Clamp(currentPower - amount, 0, maxPower);
        return PowerLevel.Level;
    }

    /// <summary>
    /// Increase battery power
    /// </summary>
    /// <param name="amount">amout to increase by</param>
    public virtual void IncreasePower(float amount)
    {
        if (!IsRunning) return;
        currentPower = Mathf.Clamp(currentPower + amount, 0, maxPower);
    }

    /// <summary>
    /// Consume some battery as rate
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator ConsumeBattery()
    {
        // do this will running
        while (IsRunning)
        {
            // wait a sec then consum battery and continue 
            yield return new WaitForSeconds(ClassData.Rate);

            currentPower = Mathf.Clamp(currentPower - consumeAmountPerRate, 0, maxPower);
        }
    }

    /// <summary>
    /// Check and set power level as needed
    /// </summary>
    protected virtual void CheckPowerLevel()
    {
        var percent = power / maxPower;
        BatteryLevel tempLevel = null;

        //list is assumed to be orderd
        foreach(var level in ClassData.PowerLevels)
        {
            if (percent > level.PowerPercent) break;
            tempLevel = level;
        }

        PowerLevel = tempLevel;
    }
    #endregion
}
