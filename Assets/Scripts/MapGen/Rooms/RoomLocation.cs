using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomLocationEnvironmentTypes
{
    Floor,
    Wall,
    Door
}

public enum RoomLocationTypes
{
    Empty,
    PlayerStartSpawn,
    EnemySpawn
}

public enum RoomLocationTraversalTypes
{
    Clear,
    Impeded,
    Blocked
}

public class RoomLocation : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int roomLocation;
    public Vector2Int Location
    {
        get { return roomLocation; }
        set { roomLocation = value; }
    }

    [SerializeField]
    protected RoomLocationEnvironmentTypes environmentLocationType;
    public RoomLocationEnvironmentTypes EnvironmentLocationType
    {
        get { return environmentLocationType; }
        set { environmentLocationType = value; }
    }

    protected RoomLocationTypes locationType;
    public RoomLocationTypes LocationType
    {
        get { return locationType; }
        set { locationType = value; }
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

        SetLocationTraversalType();
    }

    protected virtual void SetLocationTraversalType()
    {
        if (EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
        {
            LocationTraversalType = RoomLocationTraversalTypes.Blocked;
            locationTraversalCost = System.Int32.MaxValue;
        }
        else
        {
            LocationTraversalType = RoomLocationTraversalTypes.Clear;
            locationTraversalCost = 1;
        }
    }

    public bool Equals(RoomLocation location)
    {
        return this.Location.x == location.Location.x && this.Location.y == location.Location.y;
    }
}
