using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModifier : ItemModifier
{
    #region Variables
    /// <summary>
    /// Weapon this modifer is used for
    /// </summary>
    protected BaseWeapon weapon;
    #endregion

    #region Class Init
    /// <summary>
    /// Init the weapon modifier
    /// </summary>
    /// <param name="weapon"></param>
    public virtual void InitWeaponModifier(BaseWeapon weapon)
    {
        this.weapon = weapon;
    }
    #endregion
}
