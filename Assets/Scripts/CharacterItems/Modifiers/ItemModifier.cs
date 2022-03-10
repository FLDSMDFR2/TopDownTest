using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used for items that modify other items
/// </summary>
public class ItemModifier : Item
{
    #region Class Init
    protected override void PerformStart()
    {
        base.PerformStart();

        AssignModifierDetails();
    }
    #endregion

    #region Class Logic
    /// <summary>
    /// Assign modifier details to the item we will modify
    /// </summary>
    protected virtual void AssignModifierDetails(){ }
    #endregion
}
