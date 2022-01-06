using System.Collections;
using System.Collections.Generic;
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
    EnemySpawn
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

// single location within a room
public class RoomLocation
{
    protected Vector2Int roomLocation;
    public Vector2Int Location
    {
        get { return roomLocation; }
        set { roomLocation = value; }
    }

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

    protected RoomLocationTraversalTypes locationTraversalType;
    public RoomLocationTraversalTypes LocationTraversalType
    {
        get { return locationTraversalType; }
        set { locationTraversalType = value; }
    }

    // cost to travel through location
    protected int locationTraversalCost;
    public int LocationTraversalCost
    {
        get { return locationTraversalCost; }
        set { locationTraversalCost = value; }
    }

    public RoomLocation(Vector2Int location, RoomLocationEnvironmentTypes Envtype, RoomLocationTypes type)
    {
        Location = location;
        EnvironmentLocationType = Envtype;
        LocationType = type;      
    }

    protected virtual void SetLocationTraversalType()
    {
        // if the location is a wall or None (outside the room) set as blocked
        if (EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall || EnvironmentLocationType == RoomLocationEnvironmentTypes.None)
        {
            LocationTraversalType = RoomLocationTraversalTypes.Blocked;
            locationTraversalCost = System.Int32.MaxValue;
        }
        else
        {
            // default locatio to be clear
            LocationTraversalType = RoomLocationTraversalTypes.Clear;
            locationTraversalCost = 1;
        }
    }

    public bool Equals(RoomLocation location)
    {
        return this.Location.x == location.Location.x && this.Location.y == location.Location.y;
    }
}
