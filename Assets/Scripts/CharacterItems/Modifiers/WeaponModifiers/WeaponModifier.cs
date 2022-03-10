using UnityEngine;

[RequireComponent(typeof(BaseWeapon))]
public class WeaponModifier : ItemModifier
{
    #region Variables
    /// <summary>
    /// Weapon this modifer is used for
    /// </summary>
    protected BaseWeapon weapon;
    #endregion

    #region Class Init
    protected override void PerformAwake()
    {
        base.PerformAwake();

        weapon = this.GetComponent<BaseWeapon>();
    }
    #endregion

    #region Class Logic
    /// <summary>
    /// Assign modifier details to the item we will modify
    /// </summary>
    protected override void AssignModifierDetails()
    {
        weapon.SetModifierUseCosts(Data.UseCost);
        weapon.SetModifierUpKeepCosts(Data.UpKeepCost);
    }
    #endregion
}
