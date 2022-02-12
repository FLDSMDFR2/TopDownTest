using UnityEngine;

/// <summary>
/// Used to attach room items to Game Objects
/// </summary>
public class RoomItemObject : MonoBehaviour
{
    [SerializeField]
    public RoomItem Item;

    protected virtual void Start()
    {
        Item.Start();
    }
}
