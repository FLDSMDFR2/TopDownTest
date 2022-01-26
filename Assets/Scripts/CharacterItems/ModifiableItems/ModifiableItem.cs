using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents and item that can be modified
/// </summary>
public class ModifiableItem : Item
{
    /// <summary>
    /// Modifiers assoicated with this ModifiableItem
    /// </summary>
    protected List<ItemModifier> modifiers;
    /// <summary>
    /// Upkeep Cost to use this items modifiers
    /// </summary>
    protected float modifiersUpKeepCost;
    /// <summary>
    /// Cost to use this items modifiers
    /// </summary>
    protected float modifiersUseCost;

    protected virtual void Awake()
    {
        // get all itme modifiers assigned to this ModifiableItem
        modifiers = new List<ItemModifier>(GetComponents<ItemModifier>());

        InitClassDetails();

        SetModifierCosts();
        SetModifiers();
    }

    /// <summary>
    /// Init this class details
    /// </summary>
    protected virtual void InitClassDetails(){}

    /// <summary>
    /// Get the cost for all assigned modifiers
    /// </summary>
    protected virtual void SetModifierCosts()
    {
        foreach (var item in modifiers)
        {
            modifiersUpKeepCost += item.UpKeepCost();
            modifiersUseCost += item.UseCost();
        }
    }

    /// <summary>
    /// Set up info from modifiers
    /// </summary>
    protected virtual void SetModifiers(){}

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
        return modifiersUseCost + useCost;
    }

    /// <summary>
    /// Get a list of all modifiers by type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected virtual List<ItemModifier> GetModifierByType(Type type)
    {
        var retList = new List<ItemModifier>();
        foreach (var mod in modifiers)
        {
            if (mod.GetType().IsSubclassOf(type) || mod.GetType() == type)
                retList.Add(mod);
        }

        return retList;
    }
}
