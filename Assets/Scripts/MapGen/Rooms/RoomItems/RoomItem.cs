using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class RoomItem
{
    #region Properties
    [SerializeField]
    public RoomItemType Type = RoomItemType.None;

    protected int maxItemUsesInRoom;

    protected int UseCount;
    #endregion

    public RoomItem()
    {
        ConfigureSelf();
    }

    protected virtual void ConfigureSelf(){}

    public virtual void Start() { }

    #region Place Location Checks
    /// <summary>
    /// Place the item
    /// </summary>
    /// <param name="room">room to place into</param>
    /// <param name="locationKey">location to check</param>
    /// <returns>returns true if item can be placed</returns>
    public virtual bool CanPlaceItem(Room room, int2 locationKey)
    {
        if (UseCount >= maxItemUsesInRoom) return false;
        if (CheckLocation(room, locationKey))
        {
            UseCount++;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check location of room supplied return if item can be placed here
    /// </summary>
    /// <param name="room"></param>
    /// <param name="locationKey"></param>
    /// <returns></returns>
    protected virtual bool CheckLocation(Room room, int2 locationKey)
    {
        return false;
    }
    #endregion
}
