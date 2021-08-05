using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField]
    protected int mapSizeX;

    [SerializeField]
    protected int mapSizeY;

    [SerializeField]
    protected int roomSizeX;

    [SerializeField]
    protected int roomSizeY;

    [SerializeField]
    protected int numRooms;

    [SerializeField]
    protected int PercentageGoUp;
    [SerializeField]
    protected int PercentageGoDown;
    [SerializeField]
    protected int PercentageGoLeft;
    [SerializeField]
    protected int PercentageGoRight;
    [SerializeField]
    protected int deviationRate = 10;
    [SerializeField]
    protected int maxRoutes = 20;

    private int routeCount = 0;

    //---------------------------------
    // TEMP STUFF UPDATE THIS LATER
    [SerializeField]
    protected RoomConfig Config;

    public RoomBuilder builder;

    //---------------------------------

    protected Dictionary<Vector2Int, Room> map;
    protected Dictionary<Vector2Int, Room> rooms;

    // parent obejct for all rooms
    protected Transform DestParentTransform;

    // Game Difficulty selected
    protected GameDifficulty difficulty;

    public void RoomGen(Transform DestParent, GameDifficulty dif)
    {
        DestParentTransform = DestParent;
        difficulty = dif;

        SetUp();

        // start to build rooms pass null to set start room
        CreateRooms(null);

        // update all the rooms
        UpdateRooms();

        // build the rooms we have created
        builder.BuildRooms(DestParentTransform, rooms, roomSizeX, roomSizeY);
    }

    /// <summary>
    ///  Set up the room generator
    /// </summary>
    protected virtual void SetUp()
    {
        //init the rooms dictionary
        map = new Dictionary<Vector2Int, Room>();
        rooms = new Dictionary<Vector2Int, Room>();

        // init rooms locations within the dictionary
        for (int i = 0; i < mapSizeX; i++)
        {
            for (int j = 0; j < mapSizeY; j++)
            {
                var key = new Vector2Int(j, i);
                map.Add(key, null);
            }
        }
    }

    #region Create Rooms
    /// <summary>
    /// Create rooms within the map
    /// </summary>
    protected virtual void CreateRooms(Room previousRoom)
    {
        // we dont have a last room so lets build the first room
        if (previousRoom == null)
        {
            var room  = CreateFirstRoom();
            if (AddRoom(room.MapLocation, room))
            {
                // add new rooms off the start room
                NewRoute(room.MapLocation, room.MapLocation, 0);
            }
            else
            {
                // room failed some how try again
                CreateRooms(null);
            }
        }
    }

    /// <summary>
    /// Build start room
    /// </summary>
    protected virtual Room CreateFirstRoom()
    {
        // get random location on the map to start some place in the middle of the map
        var mapX = RandomGenerator.SeededRange(mapSizeX/4, mapSizeX - (mapSizeX / 4));
        var mapY = RandomGenerator.SeededRange(mapSizeY / 4, mapSizeY - (mapSizeY / 4));

        return new Room(new Vector2Int(mapX, mapY), RoomTypes.StartRoom, RoomDifficulty.Easy, Config, roomSizeX, roomSizeY);
    }

    /// <summary>
    /// Drunk crawl room layout
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="previousPos"></param>
    protected virtual void NewRoute(Vector2Int currentPos, Vector2Int previousPos, int routeLength)
    {
        if (routeCount < maxRoutes)
        {

            bool NoAvailableRoutes = false;

            routeCount++;
            while (rooms.Count <= numRooms && ++routeLength <= numRooms && !NoAvailableRoutes)
            {
                //Initialize
                bool routeUsed = false;
                var nextRoomOffset = 1;
                Vector2Int tempCurrent;

                //Go up
                if (RandomGenerator.SeededRange(1, PercentageGoUp) <= deviationRate && !(previousPos.y > currentPos.y))
                {
                    tempCurrent = new Vector2Int(currentPos.x, currentPos.y + nextRoomOffset);
                    if (map.ContainsKey(tempCurrent) && !rooms.ContainsKey(tempCurrent))
                    {
                        previousPos = currentPos;
                        currentPos = tempCurrent;
                        if (routeUsed)
                        {
                            TryCreateRoom(currentPos);
                            NewRoute(currentPos, previousPos, RandomGenerator.SeededRange(routeLength, numRooms));
                        }
                        else
                        {
                            TryCreateRoom(currentPos);
                            routeUsed = true;
                        }
                    }
                }

                //Go down
                if (RandomGenerator.SeededRange(1, PercentageGoDown) <= deviationRate && !(previousPos.y < currentPos.y))
                {
                    tempCurrent = new Vector2Int(currentPos.x, currentPos.y - nextRoomOffset);
                    if (map.ContainsKey(tempCurrent) && !rooms.ContainsKey(tempCurrent))
                    {
                        previousPos = currentPos;
                        currentPos = tempCurrent;
                        if (routeUsed)
                        {
                            TryCreateRoom(currentPos);
                            NewRoute(currentPos, previousPos, RandomGenerator.SeededRange(routeLength, numRooms));
                        }
                        else
                        {
                            TryCreateRoom(currentPos);
                            routeUsed = true;
                        }
                    }
                }

                //Go left
                if (RandomGenerator.SeededRange(1, PercentageGoLeft) <= deviationRate && !(previousPos.x < currentPos.x))
                {
                    tempCurrent = new Vector2Int(currentPos.x - nextRoomOffset, currentPos.y);
                    if (map.ContainsKey(tempCurrent) && !rooms.ContainsKey(tempCurrent))
                    {
                        previousPos = currentPos;
                        currentPos = tempCurrent;
                        if (routeUsed)
                        {
                            TryCreateRoom(currentPos);
                            NewRoute(currentPos, previousPos, RandomGenerator.SeededRange(routeLength, numRooms));
                        }
                        else
                        {
                            TryCreateRoom(currentPos);
                            routeUsed = true;
                        }
                    }
                }
                //Go right
                if (RandomGenerator.SeededRange(1, PercentageGoRight) <= deviationRate && !(previousPos.x > currentPos.x))
                {
                    tempCurrent = new Vector2Int(currentPos.x + nextRoomOffset, currentPos.y);
                    if (map.ContainsKey(tempCurrent) && !rooms.ContainsKey(tempCurrent))
                    {
                        previousPos = currentPos;
                        currentPos = tempCurrent;
                        if (routeUsed)
                        {
                            TryCreateRoom(currentPos);
                            NewRoute(currentPos, previousPos, RandomGenerator.SeededRange(routeLength, numRooms));
                        }
                        else
                        {
                            TryCreateRoom(currentPos);
                            routeUsed = true;
                        }
                    }
                }

                // we never added a room
                if (!routeUsed)
                {
                    //up
                    var tempCurrentUp = new Vector2Int(currentPos.x, currentPos.y + nextRoomOffset);
                    var tempCurrentDown = new Vector2Int(currentPos.x, currentPos.y - nextRoomOffset);
                    var tempCurrentLeft = new Vector2Int(currentPos.x - nextRoomOffset, currentPos.y);
                    var tempCurrentRight = new Vector2Int(currentPos.x + nextRoomOffset, currentPos.y);
                    if (map.ContainsKey(tempCurrentUp) && !rooms.ContainsKey(tempCurrentUp))
                    {
                        previousPos = currentPos;
                        currentPos = tempCurrentUp;
                        TryCreateRoom(currentPos);
                        routeUsed = true;
                    }
                    //down
                    else if (map.ContainsKey(tempCurrentDown) && !rooms.ContainsKey(tempCurrentDown))
                    {
                        previousPos = currentPos;
                        currentPos = tempCurrentDown;
                        TryCreateRoom(currentPos);
                        routeUsed = true;
                    }
                    //left
                    else if (map.ContainsKey(tempCurrentLeft) && !rooms.ContainsKey(tempCurrentLeft))
                    {
                        previousPos = currentPos;
                        currentPos = tempCurrentLeft;
                        TryCreateRoom(currentPos);
                        routeUsed = true;
                    }
                    //right
                    else if (map.ContainsKey(tempCurrentRight) && !rooms.ContainsKey(tempCurrentRight))
                    {
                        previousPos = currentPos;
                        currentPos = tempCurrentRight;
                        TryCreateRoom(currentPos);
                        routeUsed = true;
                    }
                }

                // we must have ended in a corner let another worker try to finish
                if (!routeUsed)
                {
                    NoAvailableRoutes = true;
                }

            }
        }
    }

    protected virtual void TryCreateRoom(Vector2Int location)
    {

        // TODO: make this random based on difficulty
        var room = new Room(location, RoomTypes.Enemy, RoomDifficulty.Easy, Config, roomSizeX, roomSizeY);

        //if the location is in the map and not a room
        if (AddRoom(location, room))
        {
            CreateRooms(room);
        }
    }


    /// <summary>
    /// Add the room to  the map and rooms dictionary
    /// </summary>
    /// <param name="key">room loc</param>
    /// <param name="r">room to add</param>
    /// <returns>if room was added</returns>
    protected virtual bool AddRoom(Vector2Int key, Room r)
    {
        // if we hit the room count limit then dont add anymore
        if (rooms.Count >= numRooms)
            return false;

        if (map.ContainsKey(key) && !rooms.ContainsKey(key))
        {
            map[key] = r;
            rooms.Add(key, r);
            return true;
        }
        return false;
    }
    #endregion

    #region Update Rooms
    protected virtual void UpdateRooms()
    {
        // add doors to rooms
        AddDoors();

        // set room Difficulty / type
        UpdateRoomsTypes();

        //Generate room  layout
        GenerateRoomLayout();
    }

    /// <summary>
    /// Add Room connections
    /// </summary>
    protected virtual void AddDoors()
    {
        Vector2Int doorLocation;
        // the key list should be in order that the rooms where added
        foreach (var key in rooms.Keys)
        {
            // location to check
            Vector2Int checkLocation = rooms[key].MapLocation + new Vector2Int(0, 1);

            //check right  if room add door
            if (map.ContainsKey(checkLocation) && map[checkLocation] != null)
            {
                doorLocation = rooms[key].RoomConvertedCenter() + new Vector2Int(0, (rooms[key].RoomSizeY / 2) - 1) ;
                CreateDoor(doorLocation, DoorSide.Right, 2, rooms[key], map[checkLocation]);
            }

            checkLocation = rooms[key].MapLocation + new Vector2Int(0, -1);

            //check left  if room add door
            if (map.ContainsKey(checkLocation) && map[checkLocation] != null)
            {
                doorLocation = rooms[key].RoomConvertedCenter() - new Vector2Int(0, rooms[key].RoomSizeY / 2);
                CreateDoor(doorLocation, DoorSide.Left, 2, rooms[key], map[checkLocation]);
            }

            checkLocation = rooms[key].MapLocation + new Vector2Int(1, 0);

            //check up  if room add door
            if (map.ContainsKey(checkLocation) && map[checkLocation] != null)
            {
                doorLocation = rooms[key].RoomConvertedCenter() + new Vector2Int((rooms[key].RoomSizeX / 2) -1, 0);
                CreateDoor(doorLocation, DoorSide.Up, 2, rooms[key], map[checkLocation]);
            }

            checkLocation = rooms[key].MapLocation + new Vector2Int(-1, 0);
            //check down  if room add door
            if (map.ContainsKey(checkLocation) && map[checkLocation] != null)
            {
                doorLocation = rooms[key].RoomConvertedCenter() - new Vector2Int(rooms[key].RoomSizeX / 2, 0);
                CreateDoor(doorLocation, DoorSide.Down, 2, rooms[key], map[checkLocation]);
            }
        }
    }

    protected virtual void CreateDoor(Vector2Int Location, DoorSide side, int size, Room fromRoom, Room toRoom)
    {
        // make sure we are not adding a duplicate door
        foreach (var d in fromRoom.Doors)
        {
            foreach (var r in d.ConnectedRooms)
            {
                if (r == toRoom) return;
            }
        }

        foreach (var d in toRoom.Doors)
        {
            foreach (var r in d.ConnectedRooms)
            {
                if (r == fromRoom) return;
            }
        }

        // create and add door
        var DoorLocations = new List<Vector2Int>();
        DoorLocations.Add(Location);
        for (int i = 0; i <= size; i++)
        {
            if (side == DoorSide.Left || side == DoorSide.Right)
            {
                DoorLocations.Add(Location + new Vector2Int(1 + i, 0));
            }
            else
            {
                DoorLocations.Add(Location + new Vector2Int(0, 1 + i));
            }
        }

        var ConnectedRooms = new List<Room>();
        ConnectedRooms.Add(fromRoom);
        ConnectedRooms.Add(toRoom);


        fromRoom.Doors.Add(new Door(DoorLocations, side, ConnectedRooms));

        DoorSide toDoorSide;
        if (side == DoorSide.Down)
            toDoorSide = DoorSide.Up;
        else if (side == DoorSide.Up)
            toDoorSide = DoorSide.Down;
        else if (side == DoorSide.Left)
            toDoorSide = DoorSide.Right;
        else
            toDoorSide = DoorSide.Left;

        toRoom.Doors.Add(new Door(DoorLocations, toDoorSide, ConnectedRooms,false));
    }

    /// <summary>
    /// set the room type and difficulty
    /// </summary>
    protected virtual void UpdateRoomsTypes()
    {
        foreach (var key in rooms.Keys)
        {

        }
    }

    /// <summary>
    /// Once we have updated the room tell the room to generate a layout
    /// </summary>
    protected virtual void GenerateRoomLayout()
    {
        foreach (var key in rooms.Keys)
        {
            rooms[key].GenerateRoomLayout();
        }
    }
    #endregion


    // return key for room based on real world location
    public virtual Vector2Int GetRoomKey(Vector2Int location)
    {
        return new Vector2Int(location.x / roomSizeX, location.y / roomSizeY);
    }

    public Dictionary<Vector2Int, Room> GetMap()
    {
        return map;
    }

    public Dictionary<Vector2Int, Room> GetRooms()
    {
        return rooms;
    }
}
