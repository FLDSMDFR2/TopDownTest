using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Generate the Map
/// </summary>
public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// Debug
    /// </summary>
    [SerializeField]
    protected bool DebugEnabled = false;

    [Header("Map Generation")]
    /// <summary>
    /// Map size X
    /// </summary>
    [SerializeField]
    protected int mapSizeX;
    public int MapSizeX
    {
        get
        {
            return mapSizeX;
        }
    }
    /// <summary>
    /// Map size y
    /// </summary>
    [SerializeField]
    protected int mapSizeY;
    public int MapSizeY
    {
        get
        {
            return mapSizeY;
        }
    }
    /// <summary>
    /// Map Chunk size X
    /// </summary>
    [SerializeField]
    protected int mapChunkSizeX;
    public int MapChunkSizeX
    {
        get
        {
            return mapChunkSizeX;
        }
    }
    /// <summary>
    /// Map Chunk size y
    /// </summary>
    [SerializeField]
    protected int mapChunkSizeY;
    public int MapChunkSizeY
    {
        get
        {
            return mapChunkSizeY;
        }
    }
    /// <summary>
    /// Percent change to get a room that is max size in a direction X or Y
    /// </summary>
    [SerializeField]
    protected int percentageLargeRoomDim;
    /// <summary>
    /// Min room size X
    /// </summary>
    [SerializeField]
    public int MinRoomSizeX;
    /// <summary>
    /// Generate room size for x based on min size x and map size x (use percentageLargeRoomDim to maybe get max size)
    /// </summary>
    protected int roomSizeX
    {
        get
        {
            // check to see if this will have a max size dim
            var largeRoomChange = RandomGenerator.SeededRange(1, 100);
            if (largeRoomChange < percentageLargeRoomDim)
                return mapSizeX;

            // we want to make sure its and even number 
            var temp = RandomGenerator.SeededRange(MinRoomSizeX, MapChunkSizeX);
            return temp % 2 != 0 ? temp + 1 : temp;
        }
    }
    /// <summary>
    /// Min room size X
    /// </summary>
    [SerializeField]
    public int MinRoomSizeY;
    /// <summary>
    /// Generate room size for y based on min size y and map size y (use percentageLargeRoomDim to maybe get max size)
    /// </summary>
    protected int roomSizeY
    { 
        get
        {
            // check to see if this will have a max size dim
            var largeRoomChange = RandomGenerator.SeededRange(1, 100);
            if (largeRoomChange < percentageLargeRoomDim)
                return mapSizeY;

            // we want to make sure its and even number 
            var temp = RandomGenerator.SeededRange(MinRoomSizeY, MapChunkSizeY);
            return temp % 2 != 0 ? temp + 1 : temp;
        }
    }
    /// <summary>
    /// Total number of rooms to generate in the map
    /// </summary>
    [SerializeField]
    protected int numRooms;
    /// <summary>
    /// Percent for map walker to go up from current location on the map when generation room locations
    /// </summary>
    [SerializeField]
    protected int percentageGoUp;
    /// <summary>
    /// Percent for map walker to go down from current location on the map when generation room locations
    /// </summary>
    [SerializeField]
    protected int percentageGoDown;
    /// <summary>
    /// Percent for map walker to go left from current location on the map when generation room locations
    /// </summary>
    [SerializeField]
    protected int percentageGoLeft;
    /// <summary>
    /// Percent for map walker to go right from current location on the map when generation room locations
    /// </summary>
    [SerializeField]
    protected int percentageGoRight;
    /// <summary>
    /// Rate to change direction
    /// </summary>
    [SerializeField]
    protected int deviationRate = 33;
    /// <summary>
    /// Max number of walkers that can be created
    /// </summary>
    [SerializeField]
    protected int maxRoutes = 20;
    /// <summary>
    /// Current number of walkers
    /// </summary>
    private int routeCount = 0;
    /// <summary>
    /// This hold a lookup for all location on the map (not all location will be used)
    /// </summary>
    protected Dictionary<int2, RoomData> map = new Dictionary<int2, RoomData>();
    /// <summary>
    /// This hold a lookup for all rooms generated within the map
    /// </summary>
    protected Dictionary<int2, RoomData> rooms = new Dictionary<int2, RoomData>(); 

    public void MapGen()
    {
        Init();
        // create room data within the map
        CreateRooms(null);
    }

    /// <summary>
    ///  Init needed values for map generation
    /// </summary>
    protected virtual void Init()
    {
        // init map dictionary locations to null
        for (int i = 0; i < MapSizeX; i++)
        {
            for (int j = 0; j < MapSizeY; j++)
            {
                var key = new int2(j, i);
                map.Add(key, null);
            }
        }
    }

    #region Debug
    void OnDrawGizmos()
    {
        if (Application.isPlaying && DebugEnabled)
        {
            // map display
            Gizmos.color = Color.red;
            foreach (var key in map.Keys)
            {
                Gizmos.DrawWireCube(new Vector3(key.x * MapSizeX + (MapSizeX / 2), 0, key.y * MapSizeY + (MapSizeY / 2)), new Vector3(MapChunkSizeX, .1f, MapChunkSizeY));
            }
        }
    }
    #endregion

    #region Create Rooms
    /// <summary>
    /// Create rooms within the map
    /// </summary>
    protected virtual void CreateRooms(RoomData previousRoom)
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
    /// Create the first room data some place in the middle of the map
    /// </summary>
    protected virtual RoomData CreateFirstRoom()
    {
        // get random location on the map to start some place in the middle of the map
        var mapX = RandomGenerator.SeededRange(MapSizeX / 4, MapSizeX - (MapSizeX / 4));
        var mapY = RandomGenerator.SeededRange(MapSizeY / 4, MapSizeY - (MapSizeY / 4));

        return new RoomData(new int2(mapX, mapY), roomSizeX, roomSizeY, MapChunkSizeX, MapChunkSizeY); ;
    }

    /// <summary>
    /// Drunk crawl room layout
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="previousPos"></param>
    protected virtual void NewRoute(int2 currentPos, int2 previousPos, int routeLength)
    {
        if (routeCount < maxRoutes)
        {

            bool NoAvailableRoutes = false;

            routeCount++;
            while (rooms.Count <= numRooms && ++routeLength <= numRooms && !NoAvailableRoutes)
            {
                //Initialize
                bool routeUsed = false;
                int2 tempCurrent;

                //Go up
                if (RandomGenerator.SeededRange(1, percentageGoUp) <= deviationRate && !(previousPos.y > currentPos.y))
                {
                    tempCurrent = currentPos + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Up];
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
                if (RandomGenerator.SeededRange(1, percentageGoDown) <= deviationRate && !(previousPos.y < currentPos.y))
                {
                    tempCurrent = currentPos + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Down];
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
                if (RandomGenerator.SeededRange(1, percentageGoLeft) <= deviationRate && !(previousPos.x < currentPos.x))
                {
                    tempCurrent = currentPos + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Left];
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
                if (RandomGenerator.SeededRange(1, percentageGoRight) <= deviationRate && !(previousPos.x > currentPos.x))
                {
                    tempCurrent = currentPos + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Right];
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
                    var tempCurrentUp = currentPos + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Up];
                    var tempCurrentDown = currentPos + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Down];
                    var tempCurrentLeft = currentPos + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Left];
                    var tempCurrentRight = currentPos + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Right];
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

    protected virtual void TryCreateRoom(int2 location)
    {
        // create room data for this location
        var roomData = new RoomData(location, roomSizeX, roomSizeY, MapChunkSizeX, MapChunkSizeY);

        //if the location is in the map and not a room
        if (AddRoom(location, roomData))
        {
            CreateRooms(roomData);
        }
    }

    /// <summary>
    /// Add the room to the map and rooms dictionary
    /// </summary>
    /// <param name="key">room loc</param>
    /// <param name="rd">roomdata to add</param>
    /// <returns>if room was added</returns>
    protected virtual bool AddRoom(int2 key, RoomData rd)
    {
        // if we hit the room count limit then dont add anymore
        if (rooms.Count >= numRooms)
            return false;

        // location is on the map and is not already a room
        if (map.ContainsKey(key) && !rooms.ContainsKey(key))
        {
            map[key] = rd;
            rooms.Add(key, rd);
            return true;
        }
        return false;
    }
    #endregion

    #region Helpers
    // return key for room Chunk based on real world location
    public virtual int2 GetRoomKey(int2 location)
    {
        return new int2(location.x / mapSizeX, location.y / mapSizeY);
    }

    public Dictionary<int2, RoomData> GetMap()
    {
        return map;
    }

    public Dictionary<int2, RoomData> GetRooms()
    {
        return rooms;
    }
    #endregion
}
