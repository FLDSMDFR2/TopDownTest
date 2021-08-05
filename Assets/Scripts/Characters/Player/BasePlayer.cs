using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : BaseCharacter
{
    protected CharacterInputController inputController;

    protected override void Awake()
    {
        base.Awake();
        inputController = GetComponent<CharacterInputController>();
        inputController.OnFirePrimary += FirePrimaryWeapon;

    }


    private void OnDestroy()
    {
        inputController.OnFirePrimary -= FirePrimaryWeapon;
    }
}
