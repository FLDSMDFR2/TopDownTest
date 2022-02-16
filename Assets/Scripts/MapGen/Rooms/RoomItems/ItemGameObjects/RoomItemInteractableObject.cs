using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemInteractableObject : RoomItemObject
{
    protected UI_RoomItemIteraction roomItemIteraction;

    protected float interactionRange;
    protected GameObject interactingCharacter;

    /// <summary>
    /// Init needed interact components
    /// </summary>
    protected override void PerformAwakeInit()
    {
        roomItemIteraction = GetComponentInChildren<UI_RoomItemIteraction>();
    }

    /// <summary>
    /// Perform interact 
    /// </summary>
    /// <param name="character">character to perfrom the interact with</param>
    public virtual void PerformInteract(BaseCharacter character)
    {
        interactionRange = character.FOV.LOSRadius;
        interactingCharacter = character.gameObject;
    }

    /// <summary>
    /// Display the interaction Icon
    /// </summary>
    public virtual void DisplayInteractIcon()
    {
        roomItemIteraction?.Show();
    }

    /// <summary>
    /// Stop displaying the interaction Icon
    /// </summary>
    public virtual void StopDisplayInteractIcon()
    {
        roomItemIteraction?.Hide();
    }

    /// <summary>
    /// If an interacting character is out of range
    /// </summary>
    /// <returns>True if out of range</returns>
    protected virtual bool IsOutOfInteractRange()
    {
        // if we dont have a character interacting with this object
        if (interactingCharacter == null) return true;
        // if the character is to far away
        if (Vector3.Distance(transform.position, interactingCharacter.transform.position) > interactionRange)
        {
            // clear the character and return out of range
            interactingCharacter = null;
            return true;
        }

        return false;
    }
}
