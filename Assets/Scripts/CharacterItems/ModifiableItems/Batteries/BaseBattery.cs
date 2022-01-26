using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBattery : ModifiableItem
{
    [Header("Base Battery")]
    /// <summary>
    /// Max power in battery
    /// </summary>
    [SerializeField]
    protected float BaseMaxPower;
    protected float maxPower;
    public float MaxPower { get { return maxPower; } }
    /// <summary>
    /// Current power in battery
    /// </summary>
    [SerializeField]
    protected float power = -1;
    protected float currentPower
    {
        set
        {
            power = value;
            if (character != null)
                UIEvents.RaisePowerUpdateEvent(character.ID, power, MaxPower);
        }
        get { return power; }
    }
    /// <summary>
    /// the rate at which we consume power
    /// </summary>
    protected float Rate = 1;
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
    /// Init this class details
    /// </summary>
    protected override void InitClassDetails()
    {
        base.InitClassDetails();

        maxPower = BaseMaxPower;
    }

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
    /// <returns>True if can consume</returns>
    public virtual bool ConsumePower(float amount)
    {
        if (currentPower < amount) return false;
        if (!IsRunning) return false;

        currentPower = Mathf.Clamp(currentPower - amount, 0, maxPower);
        return true;
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
            yield return new WaitForSeconds(Rate);

            currentPower = Mathf.Clamp(currentPower - consumeAmountPerRate, 0, maxPower);
        }
    }

    /// <summary>
    /// Set up info from modifiers
    /// </summary>
    protected override void SetModifiers()
    {
        var mods = GetModifierByType(typeof(BatteryModifier));
        foreach (var mod in mods)
        {
            var batterMod =  mod as BatteryModifier;
            maxPower += batterMod.Power;
        }
    }
}
