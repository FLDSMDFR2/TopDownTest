using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use the room map data that was generated in the map builder to generate room details
/// </summary>
public class RoomGenerator : MonoBehaviour
{
    /// <summary>
    /// Debug
    /// </summary>
    [SerializeField]
    protected bool DebugEnabled = false;

    [Header("Room Generation")]
    /// <summary>
    /// Min size of a hallway between rooms
    /// </summary>
    [SerializeField]
    protected int minRoomConnectionSize;
    /// <summary>
    /// Parent obejct for all rooms
    /// </summary>
    protected Transform destParentTransform;
    /// <summary>
    /// Map the was generated
    /// </summary>
    protected MapGenerator mapGenerator;
    /// <summary>
    /// List of room created
    /// </summary>
    protected List<Room> rooms = new List<Room>();

    //---------------------------------
    // TODO : REMOVE LATER
    [SerializeField]
    protected RoomPrefabConfig Config;
    //---------------------------------

    #region Debug
    void OnDrawGizmos()
    {
        if (Application.isPlaying && DebugEnabled)
        {
            // room display
            foreach (var r in rooms)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(new Vector3(r.Data.RoomConvertedCenter().x, 1f, r.Data.RoomConvertedCenter().y), new Vector3(4, 4, 4));

                foreach (var d in r.Data.Doors)
                {
                    var direction = Vector3.zero;
                    if (d.Side == DoorSide.Left)
                        direction = Vector3.left;
                    else if (d.Side == DoorSide.Right)
                        direction = Vector3.right;
                    else if (d.Side == DoorSide.Up)
                        direction = Vector3.forward;
                    else if (d.Side == DoorSide.Down)
                        direction = Vector3.back;

                    Gizmos.DrawRay(new Vector3(r.Data.RoomConvertedCenter().x, .5f, r.Data.RoomConvertedCenter().y), direction * 25);
                }

                Gizmos.color = Color.green;
                foreach (var rl in r.Data.RoomLocations.Keys)
                {
                    if (r.Data.RoomLocations[rl].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door)
                        Gizmos.DrawWireCube(new Vector3(rl.x, 1f, rl.y), new Vector3(1, 1, 1));
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// Based on MapGenerator supplied generate room details within map
    /// </summary>
    /// <param name="mg"></param>
    public virtual void RoomGen(MapGenerator mg, Transform DestParent)
    {
        mapGenerator = mg;
        // set parent transform for the map
        destParentTransform = DestParent;

        // add Room Connections
        AddRoomConnections();

        // Create and build rooms base on map data
        BuildRooms();

        //Generate room  layout
        GenerateRoomLayout();
    }

    #region AddRoomConnections
    /// <summary>
    /// Add Room connections
    /// </summary>
    protected virtual void AddRoomConnections()
    {
        // the key list should be in order that the rooms where added
        var mapRooms = mapGenerator.GetRooms();
        var map = mapGenerator.GetMap();
        foreach (var key in mapRooms.Keys)
        {
            // location to check
            Vector2Int checkLocation = mapRooms[key].MapLocation + new Vector2Int(0, 1);

            //check UP if room add door
            if (map.ContainsKey(checkLocation) && map[checkLocation] != null)
            {
                CreateRoomConnection(DoorSide.Up, mapRooms[key], map[checkLocation]);
            }

            //check DOWN if room add door
            checkLocation = mapRooms[key].MapLocation + new Vector2Int(0, -1);
            if (map.ContainsKey(checkLocation) && map[checkLocation] != null)
            {
                CreateRoomConnection(DoorSide.Down, mapRooms[key], map[checkLocation]);
            }

            //check RIGHT if room add door
            checkLocation = mapRooms[key].MapLocation + new Vector2Int(1, 0);
            if (map.ContainsKey(checkLocation) && map[checkLocation] != null)
            {
                CreateRoomConnection(DoorSide.Right, mapRooms[key], map[checkLocation]);
            }

            //check LEFT if room add door
            checkLocation = mapRooms[key].MapLocation + new Vector2Int(-1, 0);
            if (map.ContainsKey(checkLocation) && map[checkLocation] != null)
            {
                CreateRoomConnection(DoorSide.Left, mapRooms[key], map[checkLocation]);
            }
        }
    }

    protected virtual void CreateRoomConnection(DoorSide side, RoomData fromRoom, RoomData toRoom)
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

        var ConnectedRooms = new List<RoomData>();
        ConnectedRooms.Add(fromRoom);
        ConnectedRooms.Add(toRoom);

        // set the max connection size base on the room dims default to the smallest room dim
        var maxConnectionSize = mapGenerator.MinRoomSizeX > mapGenerator.MinRoomSizeY ? mapGenerator.MinRoomSizeY : mapGenerator.MinRoomSizeX;
        if (side == DoorSide.Up || side == DoorSide.Down)
            maxConnectionSize = fromRoom.RoomSizeX > toRoom.RoomSizeX ? toRoom.RoomSizeX : fromRoom.RoomSizeX;
        else
            maxConnectionSize = fromRoom.RoomSizeY > toRoom.RoomSizeY ? toRoom.RoomSizeY : fromRoom.RoomSizeY;

        var roomConnectionSide = RandomGenerator.SeededRange(minRoomConnectionSize, (maxConnectionSize / 2) - 1);


        // TODO: maybe move this around so its not always in the middle of the room
        fromRoom.Doors.Add(new DoorData(fromRoom.RoomConvertedCenter(), side, ConnectedRooms, roomConnectionSide));

        DoorSide toDoorSide;
        if (side == DoorSide.Down)
            toDoorSide = DoorSide.Up;
        else if (side == DoorSide.Up)
            toDoorSide = DoorSide.Down;
        else if (side == DoorSide.Left)
            toDoorSide = DoorSide.Right;
        else
            toDoorSide = DoorSide.Left;

        // TODO: maybe move this around so its not always in the middle of the room
        toRoom.Doors.Add(new DoorData(fromRoom.RoomConvertedCenter(), toDoorSide, ConnectedRooms, roomConnectionSide));
    }
    #endregion

    #region BuildRooms
    protected virtual void BuildRooms()
    {
        UpdateRoomsTypes();

        CreateRooms();
    }

    /// <summary>
    /// Create Rooms / types as needed
    /// </summary>
    protected virtual void UpdateRoomsTypes()
    {
        //TODO : MAKE THIS BETTER
        var count = 0;

        var mapRooms = mapGenerator.GetRooms();
        foreach (var key in mapRooms.Keys)
        {
            if (count <= 0)
            {
                //set first room to start room
                mapRooms[key].RoomType = RoomTypes.StartRoom;
                mapRooms[key].PrefabConfig = Config;
                mapRooms[key].Difficulty = RoomDifficulty.Easy;
            }
            else
            {
                //set all the rest of the room to an enemy room
                mapRooms[key].RoomType = RoomTypes.Enemy;
                mapRooms[key].PrefabConfig = Config;
                mapRooms[key].Difficulty = RoomDifficulty.Easy;
            }

            count++;
        }
    }

    protected virtual void CreateRooms()
    {
        int roomId = 1;
        var mapRooms = mapGenerator.GetRooms();
        foreach (var key in mapRooms.Keys)
        {
            GenerateRoom(mapRooms[key], roomId);
            roomId++;
        }
    }

    protected virtual void GenerateRoom(RoomData data, int roomId)
    {
        var go = new GameObject("Room " + roomId++);
        go.transform.parent = destParentTransform;
        Room room;

        switch (data.RoomType)
        {
            case RoomTypes.StartRoom:
                room = go.AddComponent<StartRoom>();
                break;
            case RoomTypes.Enemy:
                room = go.AddComponent<EnemyRoom>();
                break;
            default:
                room = go.AddComponent<Room>();
                break;
        }
        if (room != null)
        {
            // add data to the room then add to created list
            room.Data = data;
            rooms.Add(room);
        }
    }
    #endregion

    #region GenerateRoomLayout
    /// <summary>
    /// Once we have updated the room tell the room to generate a layout
    /// </summary>
    protected virtual void GenerateRoomLayout()
    {
        foreach (var r in rooms)
        {
            r.GenerateRoomLayout();
        }
    }
    #endregion
}
