using UnityEngine;

[RequireComponent(typeof(CharacterInputController))]
public class BasePlayer : BaseCharacter
{
    [Header("Base Player")]

    protected CharacterInputController inputController;
    protected override void Awake()
    {
        base.Awake();
        inputController = GetComponent<CharacterInputController>();
        inputController.OnFirePrimary += FireWeapon;
    }

    private void OnDestroy()
    {
        inputController.OnFirePrimary -= FireWeapon;
    }
}
