﻿using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Environment for this location ie walls, floors
/// </summary>
public enum RoomLocationEnvironmentTypes
{
    None,
    Floor,
    Wall,
    Door
}

/// <summary>
/// Location type, if there is an object at this location
/// </summary>
public enum RoomLocationTypes
{
    Empty,
    Filled,
    PlayerStartSpawn,
    Item
}

/// <summary>
/// Travel type / cost for this location
/// </summary>
public enum RoomLocationTraversalTypes
{
    Clear,
    Impeded,
    Blocked
}

/// <summary>
/// single location within a room
/// </summary>
[Serializable]
public class RoomLocationData
{
    #region Properties
    /// <summary>
    /// Location key within the room
    /// </summary>
    [SerializeField]
    protected int2 roomLocation;
    public int2 Location
    {
        get { return roomLocation; }
        set { roomLocation = value; }
    }
    /// <summary>
    /// What type of environment obj to place here
    /// </summary>
    [SerializeField]
    protected RoomLocationEnvironmentTypes environmentLocationType;
    public RoomLocationEnvironmentTypes EnvironmentLocationType
    {
        get { return environmentLocationType; }
        set
        {
            environmentLocationType = value;
            SetLocationTraversalType();
        }
    }
    /// <summary>
    /// What type of object is placed here (if any)
    /// </summary>
    [SerializeField]
    protected RoomLocationTypes locationType;
    public RoomLocationTypes LocationType
    {
        get { return locationType; }
        set
        {
            locationType = value;
            SetLocationTraversalType();
        }
    }
    /// <summary>
    /// Cost to travers this location (mostly for AI?)
    /// </summary>
    [SerializeField]
    protected RoomLocationTraversalTypes locationTraversalType;
    public RoomLocationTraversalTypes LocationTraversalType
    {
        get { return locationTraversalType; }
        set { locationTraversalType = value; }
    }
    /// <summary>
    ///  Cost to travel through location (should be tied to the type, mostly for AI)
    /// </summary>
    [SerializeField]
    protected int locationTraversalCost;
    public int LocationTraversalCost
    {
        get { return locationTraversalCost; }
    }

    /// <summary>
    /// if this room location has an item this will be set
    /// </summary>
    protected RoomItem item;
    public RoomItem Item
    {
        get { return item; }
        set { item = value; }
    }
    #endregion

    public RoomLocationData(int2 location, RoomLocationEnvironmentTypes Envtype, RoomLocationTypes type)
    {
        Location = location;
        EnvironmentLocationType = Envtype;
        LocationType = type;      
    }

    /// <summary>
    /// Set / Update traversal cost and type based on environment type / roomLocation type of the location
    /// </summary>
    protected virtual void SetLocationTraversalType()
    {
        // if the location is a wall or None (outside the room) set as blocked
        if (EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall || EnvironmentLocationType == RoomLocationEnvironmentTypes.None)
        {
            LocationTraversalType = RoomLocationTraversalTypes.Blocked;
            locationTraversalCost = System.Int32.MaxValue;
        }
        else if (LocationType == RoomLocationTypes.Item)
        {
            // Set impeded path value if there are items at this loction
            LocationTraversalType = RoomLocationTraversalTypes.Impeded;
            locationTraversalCost = 10;
        }
        else
        {
            // default locatio to be clear
            LocationTraversalType = RoomLocationTraversalTypes.Clear;
            locationTraversalCost = 1;
        }
    }

    /// <summary>
    /// If this location is passable when path finding
    /// </summary>
    /// <returns></returns>
    public virtual bool DetermineIsPassable()
    {
        if (LocationTraversalType == RoomLocationTraversalTypes.Clear || LocationTraversalType == RoomLocationTraversalTypes.Impeded)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Helper to check if this room location is the same as supplied
    /// </summary>
    /// <param name="location">location to check against</param>
    /// <returns></returns>
    public bool Equals(RoomLocationData location)
    {
        return this.Location.x == location.Location.x && this.Location.y == location.Location.y;
    }
}
