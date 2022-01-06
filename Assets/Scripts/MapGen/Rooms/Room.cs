using System.Collections;
using System.Collections.Generic;
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

    protected int mapSizeX;
    public int MapSizeX
    {
        get { return mapSizeX; }
        set { mapSizeX = value; }
    }

    protected int mapSizeY;
    public int MapSizeY
    {
        get { return mapSizeY; }
        set { mapSizeY = value; }
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

    public Room(Vector2Int location, RoomTypes type, RoomDifficulty roomDifficulty, RoomConfig roomConfig, int sizeX, int sizeY, int mSizeX, int mSizeY)
    {
        MapLocation = location;
        RoomType = type;
        difficulty = roomDifficulty;
        config = roomConfig;
        RoomSizeX = sizeX;
        RoomSizeY = sizeY;
        MapSizeX = mSizeX;
        MapSizeY = mSizeY;

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


        //set origin to the map origin for the room
        var origin = MapRoomConvertedOrigin();

        // loop over the whole room map set all location to None by default, also check for hallways as this will be outside the room but within the map
        for (int i = origin.x; i < origin.x + MapSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + MapSizeY; j++)
            {
                var key = new Vector2Int(i, j);

                if (GenerateHallway(key))
                {
                    // if this is a hallway mark it as floor
                    roomLocations.Add(key, new RoomLocation(key, RoomLocationEnvironmentTypes.Floor, RoomLocationTypes.Empty));
                }
                else
                {
                    // add all locations as None
                    roomLocations.Add(key, new RoomLocation(key, RoomLocationEnvironmentTypes.None, RoomLocationTypes.Empty));
                }
            }
        }

        // update origin to the rooms origin
        origin = RoomConvertedOrigin();

        // loop over the room locations
        for (int i = origin.x; i < origin.x + RoomSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + RoomSizeY; j++)
            {
                var key = new Vector2Int(i, j);

                // Check to place walls and doors if we dont place a wall or door then place a floor
                if (!GenerateDoors(key, origin.x, origin.y) && roomLocations.ContainsKey(key))
                {
                    // update location to floor as its should be in the room and not a wall or door
                    roomLocations[key].EnvironmentLocationType = RoomLocationEnvironmentTypes.Floor;
                }
            }
        }

        origin = MapRoomConvertedOrigin();

        // loop over the whole room map set all location to None by default, also check for hallways as this will be outside the room but within the map
        for (int i = origin.x; i < origin.x + MapSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + MapSizeY; j++)
            {
                var key = new Vector2Int(i, j);

                if (GenerateWall(key))
                {
                    // if this is a hallway mark it as floor
                    roomLocations[key].EnvironmentLocationType = RoomLocationEnvironmentTypes.Wall;
                }
            }
        }

        // add room type specific details
        BuildRoomByType();
    }

    #region Generate Room Helpers
    protected virtual bool GenerateDoors(Vector2Int key, int startX, int StartY)
    {
        //  check if we are the outer edge of the room
        if (key.x == startX || key.y == StartY || key.x == (startX + RoomSizeX) - 1 || key.y == (StartY + RoomSizeY) - 1)
        {
            // get the side we are checking for door
            DoorSide side = DoorSide.None;
            if (key.x == startX)
                side = DoorSide.Left;
            else if (key.y == StartY)
                side = DoorSide.Down;
            else if (key.x == (startX + RoomSizeX) - 1)
                side = DoorSide.Right;
            else if (key.y == (StartY + RoomSizeY) - 1)
                side = DoorSide.Up;

            // if the location is a door then place it
            if (GenerateDoor(key, side) && roomLocations.ContainsKey(key))
            {
                // update this location to be a door
                roomLocations[key].EnvironmentLocationType = RoomLocationEnvironmentTypes.Door;
            }
            return true;
        }

        return false;
    }
    /// <summary>
    /// Return if the location provided is a hall location
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    protected virtual bool GenerateHallway(Vector2Int location)
    {
        var origin = RoomConvertedOrigin();

        // check if the location is outside the room
        if (location.x < origin.x || location.y < origin.y || location.x > (origin.x + RoomSizeX) - 1 || location.y > (origin.y + RoomSizeY) - 1)
        {
            // location is outside the room but on the room map
            if (location.x < origin.x)
                return CheckDoorForHallway(location, DoorSide.Left);
            if (location.x > (origin.x + RoomSizeX) - 1)
                return CheckDoorForHallway(location, DoorSide.Right);
            if (location.y < origin.y)
                return CheckDoorForHallway(location, DoorSide.Down);
            if (location.y > (origin.y + RoomSizeY) - 1)
                return CheckDoorForHallway(location, DoorSide.Up);
        }

        return false;
    }

    protected virtual bool CheckDoorForHallway(Vector2Int location, DoorSide side)
    {
        foreach (var d in doors)
        {
            if (d.Side == side)
            {
                if (side == DoorSide.Left || side == DoorSide.Right)
                {
                    // if the location is in the window of the door
                    if (location.y <= d.ConnectionPoint.y + d.Size && location.y >= d.ConnectionPoint.y - d.Size)
                        return true;
                }
                else
                {
                    // if the location is in the window of the door
                    if (location.x <= d.ConnectionPoint.x + d.Size && location.x >= d.ConnectionPoint.x - d.Size)
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// If location supplied is a door location
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    protected virtual bool GenerateDoor(Vector2Int location, DoorSide side)
    {
        foreach (var d in doors)
        {
            if (d.Side == side)
            {
                if (side == DoorSide.Left || side == DoorSide.Right)
                {
                    // if the location is in the window of the door
                    if (location.y <= d.ConnectionPoint.y + d.Size && location.y >= d.ConnectionPoint.y - d.Size)
                        return true;
                }
                else
                {
                    // if the location is in the window of the door
                    if (location.x <= d.ConnectionPoint.x + d.Size && location.x >= d.ConnectionPoint.x - d.Size)
                        return true;
                }
            }
        }

        return false;
    }

    protected virtual bool GenerateWall(Vector2Int location)
    {
        // if we are outside the room
        if (roomLocations[location].EnvironmentLocationType == RoomLocationEnvironmentTypes.None)
        {
            //check if we are next to a floor or door if so then we need to be a wall

            //right
            var checkLoc = location + new Vector2Int(1, 0);
            if (roomLocations.ContainsKey(checkLoc) &&
                (roomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                roomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;

            //left
            checkLoc = location + new Vector2Int(-1, 0);
            if (roomLocations.ContainsKey(checkLoc) &&
               (roomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                roomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;

            //up
            checkLoc = location + new Vector2Int(0, 1);
            if (roomLocations.ContainsKey(checkLoc) &&
                (roomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                roomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;

            //down
            checkLoc = location + new Vector2Int(0, -1);
            if (roomLocations.ContainsKey(checkLoc) &&
                (roomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                roomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;
        }
        return false;
    }
    #endregion

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
    public virtual Vector2Int MapRoomConvertedOrigin()
    {
        return (MapLocation * new Vector2Int(mapSizeX, mapSizeY));
    }

    /// <summary>
    /// Returns 0,0 location for room bottom corner
    /// </summary>
    /// <returns></returns>
    public virtual Vector2Int RoomConvertedOrigin()
    {     
        return (RoomConvertedCenter() - new Vector2Int((roomSizeX / 2), (roomSizeY / 2)));
    }

    /// <summary>
    /// get the center of the room location within the map
    /// </summary>
    /// <returns></returns>
    public virtual Vector2Int RoomConvertedCenter()
    {
        return (MapLocation * new Vector2Int(mapSizeX, mapSizeY)) + new Vector2Int((mapSizeX / 2), (mapSizeY / 2));
    }
    #endregion
}
