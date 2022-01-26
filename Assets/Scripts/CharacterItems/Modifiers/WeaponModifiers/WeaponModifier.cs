using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModifier : ItemModifier
{
    /// <summary>
    /// Weapon this modifer is used for
    /// </summary>
    protected BaseWeapon weapon;

    /// <summary>
    /// Init the weapon modifier
    /// </summary>
    /// <param name="weapon"></param>
    public virtual void InitWeaponModifier(BaseWeapon weapon)
    {
        this.weapon = weapon;
    }
}
