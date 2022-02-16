using UnityEngine;

[RequireComponent(typeof(CharacterInputController))]
public class BasePlayer : BaseCharacter
{
    [Header("Base Player")]
    protected CharacterInputController inputController;

    protected RoomItemInteractableObject interactItem;

    protected override void Awake()
    {
        base.Awake();

        inputController = GetComponent<CharacterInputController>();
        inputController.OnFirePrimary += FireWeapon;
        inputController.OnInteract += OnInteract;
    }

    protected virtual void Update()
    {
        CheckForInteract();
    }

    #region Interaction
    protected virtual void OnInteract()
    {
        if (interactItem == null) return;

        interactItem.PerformInteract(this);
    }

    protected virtual void CheckForInteract()
    {
        FOV.PerformFOVCheck();
        if (FOV.CanSee && FOV.Target != null)
        {
            var item = FOV.Target.gameObject?.GetComponent<RoomItemInteractableObject>();
            if (item != null)
            {
                interactItem = item;
                interactItem.DisplayInteractIcon();
                return;
            }
        }

        if (interactItem != null)
        {
            interactItem.StopDisplayInteractIcon();
            interactItem = null;
        }
    }
    #endregion

    protected override void OnDestroy()
    {
        inputController.OnFirePrimary -= FireWeapon;
        inputController.OnInteract -= OnInteract;
    }
}
