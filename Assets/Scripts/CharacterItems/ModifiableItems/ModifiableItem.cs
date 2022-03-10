using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents and item that can be modified
/// </summary>
public class ModifiableItem : Item
{
    #region Variables
    /// <summary>
    /// Upkeep Cost to use this items with modifiers
    /// </summary>
    protected float modifiersUpKeepCost;
    /// <summary>
    /// Cost to use this items with modifiers
    /// </summary>
    protected float modifiersUseCost;
    #endregion

    #region Item Init
    protected override void PerformAwake()
    {
        base.PerformAwake();

        InitClassDetails();
    }
    #endregion

    #region Item Overrides
    /// <summary>
    /// Cost to Up keep
    /// </summary>
    /// <returns></returns>
    public override float UpKeepCost()
    {
        return modifiersUpKeepCost + base.UpKeepCost();
    }

    /// <summary>
    /// Cost to Use
    /// </summary>
    /// <returns></returns>
    public override float UseCost()
    {
        return modifiersUseCost + Data.UseCost;
    }
    #endregion

    #region Class Logic
    /// <summary>
    /// Init this class details
    /// </summary>
    protected virtual void InitClassDetails() { }

    /// <summary>
    /// Set Modifier Use Costs
    /// </summary>
    /// <param name="cost"></param>
    public virtual void SetModifierUseCosts(float cost)
    {
        modifiersUseCost += cost;
    }

    /// <summary>
    /// Set Modifier UpKeep Costs
    /// </summary>
    /// <param name="cost"></param>
    public virtual void SetModifierUpKeepCosts(float cost)
    {
        modifiersUpKeepCost += cost;
    }
    #endregion
}
