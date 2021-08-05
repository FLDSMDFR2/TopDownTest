using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public enum RoomTypes
{
    StartRoom,
    Empty,
    Enemy,
    Boss,
    Loot,
    Secret
}

public enum RoomDifficulty
{
    Easy,
    Medium,
    Hard,
    Insane
}

public class Room
{
    protected Vector2Int mapLocation;
    public Vector2Int MapLocation
    {
        get { return mapLocation; }
        set { mapLocation = value; }
    }


    protected RoomTypes roomType;
    public RoomTypes RoomType
    {
        get { return roomType; }
        set { roomType = value; }
    }

    protected int roomSizeX;
    public int RoomSizeX
    {
        get { return roomSizeX; }
        set { roomSizeX = value; }
    }

    protected int roomSizeY;
    public int RoomSizeY
    {
        get { return roomSizeY; }
        set { roomSizeY = value; }
    }

    protected Dictionary<Vector2Int, RoomLocation> roomLocations;
    public Dictionary<Vector2Int, RoomLocation> RoomLocations
    {
        get { return roomLocations; }
    }

    protected RoomConfig config;
    public RoomConfig Config
    {
        get { return config; }
    }

    protected RoomDifficulty difficulty;
    public RoomDifficulty Difficulty
    {
        get { return difficulty; }
    }

    protected List<Door> doors;
    public List<Door> Doors
    {
        get { return doors; }
    }

    public Room(Vector2Int location, RoomTypes type, RoomDifficulty roomDifficulty, RoomConfig roomConfig, int sizeX, int sizeY)
    {
        MapLocation = location;
        RoomType = type;
        difficulty = roomDifficulty;
        config = roomConfig;
        RoomSizeX = sizeX;
        RoomSizeY = sizeY;

        doors = new List<Door>();
    }

    public virtual void GenerateRoomLayout()
    {
        GenerateRoom();
    }

    #region Room Generation

    protected virtual void GenerateRoom()
    {
        roomLocations = new Dictionary<Vector2Int, RoomLocation>();

        // init rooms locations within the dictionary to all floor
        var x = RoomConvertedOrigin().x;
        var y = RoomConvertedOrigin().y;

        for (int i = x; i < x + RoomSizeX; i++)
        {
            for (int j = y; j < y + RoomSizeY; j++)
            {
                var key = new Vector2Int(i, j);

                // Check to place walls and doors if we dont place a wall or door then place a floor
                if (!GenerateWallsAndDoors(key,x,y))
                {
                    roomLocations.Add(key, new RoomLocation(key, RoomLocationEnvironmentTypes.Floor, RoomLocationTypes.Empty));
                }

            }
        }

        // add room type specific details
        BuildRoomByType();
    }

    protected virtual bool GenerateWallsAndDoors(Vector2Int key, int startX, int StartY)
    {
        //  check if we are the outer edge of the room
        if (key.x == startX || key.y == StartY || key.x == (startX + RoomSizeX) - 1 || key.y == (StartY + RoomSizeY) - 1)
        {
            // if the location is a door then place it
            if (IsDoor(key))
            {
                roomLocations.Add(key, new RoomLocation(key, RoomLocationEnvironmentTypes.Door, RoomLocationTypes.Empty));
            }
            else if (CheckForNoWallDoor(key, startX, StartY))
            {
                // we dont need to place a wall or door so return and place a floor
                return false;
            }
            else
            {
                // we are an outer edge so place a wall
                roomLocations.Add(key, new RoomLocation(key, RoomLocationEnvironmentTypes.Wall, RoomLocationTypes.Empty));
            }
            return true;
        }

        return false;
    }
    protected virtual bool CheckForNoWallDoor(Vector2Int key, int startX, int StartY)
    { 
        //check for corners
        if ((key.x == startX && key.y == StartY) || (key.x == (startX + RoomSizeX) - 1 && key.y == (StartY + RoomSizeY) - 1) ||
           (key.x == startX && key.y == (StartY + RoomSizeY) - 1) || (key.y == StartY && key.x == (startX + RoomSizeX) - 1))
        {
            return false;
        }

        //bottom
        if (key.x == startX)
        {
            foreach(var d in doors)
            {
                if (d.Side == DoorSide.Down && !d.IsMyDoor) return true;
            }
        }
        //left
        else  if (key.y == StartY)
        {
            foreach (var d in doors)
            {
                if (d.Side == DoorSide.Left && !d.IsMyDoor) return true;
            }
        }
        //top
        else if (key.x == (startX + RoomSizeX) - 1)
        {
            foreach (var d in doors)
            {
                if (d.Side == DoorSide.Up && !d.IsMyDoor) return true;
            }
        }
        //right
        else if (key.y == (StartY + RoomSizeY) - 1)
        {
            foreach (var d in doors)
            {
                if (d.Side == DoorSide.Right && !d.IsMyDoor) return true;
            }
        }

        return false;
    }

    protected virtual void BuildRoomByType()
    {

        GenerateRoomObstacles();

        switch (roomType)
        {
            case RoomTypes.StartRoom:
                GenerateStartRoomType();
                break;
            case RoomTypes.Empty:
                //do nothing
                break;
            case RoomTypes.Enemy:
                GenerateEnemyRoomType();
                break;
            default:
                TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "Room Type not found " + roomType.ToString());
                break;
        }
    }

    #region Room Obstacles
    protected virtual void GenerateRoomObstacles()
    {
        // just load obsticals 
    }
    #endregion

    #region StartRoom
    protected virtual void GenerateStartRoomType()
    {
        var startLocFound = false;
        while(!startLocFound)
        {
            // find random location  and check if we can set as spawn
            var location = new Vector2Int(RandomGenerator.SeededRange(RoomConvertedOrigin().x, RoomConvertedOrigin().x + roomSizeX), RandomGenerator.SeededRange(RoomConvertedOrigin().y, RoomConvertedOrigin().y + roomSizeY));

            if (RoomLocations.ContainsKey(location) &&
                RoomLocations[location].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor &&
               RoomLocations[location].LocationType == RoomLocationTypes.Empty)
            {
                // if the location is a floor and is empty set as spwan location
                RoomLocations[location].LocationType = RoomLocationTypes.PlayerStartSpawn;
                startLocFound = true;
            }

        }
    }
    #endregion

    #region Enemy
    protected virtual void GenerateEnemyRoomType()
    {
        var locFound = false;
        while (!locFound)
        {
            // find random location  and check if we can set as spawn
            var location = new Vector2Int(RandomGenerator.SeededRange(RoomConvertedOrigin().x, RoomConvertedOrigin().x + roomSizeX), RandomGenerator.SeededRange(RoomConvertedOrigin().y, RoomConvertedOrigin().y + roomSizeY));

            if (RoomLocations.ContainsKey(location) &&
                RoomLocations[location].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor &&
               RoomLocations[location].LocationType == RoomLocationTypes.Empty)
            {
                // if the location is a floor and is empty set as enemy spwan location
                RoomLocations[location].LocationType = RoomLocationTypes.EnemySpawn;
                locFound = true;
            }

        }
    }
    #endregion

    #endregion

    #region Helpers
    /// <summary>
    /// Returns 0,0 location for room bottom  left corner
    /// </summary>
    /// <returns></returns>
    public virtual Vector2Int RoomConvertedOrigin()
    {     
        return (MapLocation * new Vector2Int(roomSizeX, roomSizeY));
    }

    /// <summary>
    /// get the map center
    /// </summary>
    /// <returns></returns>
    public virtual Vector2Int RoomConvertedCenter()
    {
        return (MapLocation * new Vector2Int(roomSizeX, roomSizeY)) + new Vector2Int((roomSizeX / 2), (roomSizeY / 2));
    }

    /// <summary>
    /// If location supplied is a door location
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    protected virtual bool IsDoor(Vector2Int location)
    {
        foreach (var d in doors)
        {
            foreach (var l in d.DoorLocations)
            {
                if (l.x == location.x && l.y == location.y)
                    return true;
            }
        }

        return false;
    }
    #endregion
}
