using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Type of room
/// </summary>
public enum RoomTypes
{
    None,
    Test,
    Empty,
    StartRoom,
    Enemy,
    Boss,
    Loot,
    Secret
}

/// <summary>
/// Difficulty of the room
/// </summary>
public enum RoomDifficulty
{
    Easy,
    Medium,
    Hard,
    Insane
}

[Serializable]
public class RoomData
{
    /// <summary>
    /// Location of the room with then the Map
    /// </summary>
    [SerializeField]
    protected int2 mapLocation;
    public int2 MapLocation
    {
        get { return mapLocation; }
        set { mapLocation = value; }
    }
    /// <summary>
    /// Room Type
    /// </summary>
    [SerializeField]
    protected RoomTypes roomType;
    public RoomTypes RoomType
    {
        get { return roomType; }
        set { roomType = value; }
    }
    /// <summary>
    /// Map size X
    /// </summary>
    [SerializeField]
    protected int mapSizeX;
    public int MapSizeX
    {
        get { return mapSizeX; }
        set { mapSizeX = value; }
    }
    /// <summary>
    /// Map size Y
    /// </summary>
    [SerializeField]
    protected int mapSizeY;
    public int MapSizeY
    {
        get { return mapSizeY; }
        set { mapSizeY = value; }
    }
    /// <summary>
    /// Size of this room X
    /// </summary>
    [SerializeField]
    protected int roomSizeX;
    public int RoomSizeX
    {
        get { return roomSizeX; }
        set { roomSizeX = value; }
    }
    /// <summary>
    /// Size of this room Y
    /// </summary>
    [SerializeField]
    protected int roomSizeY;
    public int RoomSizeY
    {
        get { return roomSizeY; }
        set { roomSizeY = value; }
    }
    /// <summary>
    /// List of all locations within this room
    /// </summary>
    [SerializeField]
    protected Dictionary<int2, RoomLocationData> roomLocations;
    public Dictionary<int2, RoomLocationData> RoomLocations
    {
        get { return roomLocations; }
        set { roomLocations = value; }
    }

    /// <summary>
    /// Configuration of the room (used for rendering location types)
    /// </summary>
    [SerializeField]
    protected RoomPrefabConfig prefabConfig;
    public RoomPrefabConfig PrefabConfig
    {
        get { return prefabConfig; }
        set { prefabConfig = value; }
    }
    /// <summary>
    /// Difficulty of room (if enemies)
    /// </summary>
    [SerializeField]
    protected RoomDifficulty difficulty;
    public RoomDifficulty Difficulty
    {
        get { return difficulty; }
        set { difficulty = value; }
    }
    /// <summary>
    /// List of doors that connect to other rooms 
    /// </summary>
    [SerializeField]
    protected List<DoorData> doors;
    public List<DoorData> Doors
    {
        get { return doors; }
        set { doors = value; }
    }

    public RoomData(int2 location, int sizeX, int sizeY, int mSizeX, int mSizeY)
    {
        MapLocation = location;
        RoomSizeX = sizeX;
        RoomSizeY = sizeY;
        MapSizeX = mSizeX;
        MapSizeY = mSizeY;

        doors = new List<DoorData>();
    }

    #region Helpers
    /// <summary>
    /// Returns 0,0 location for room locations map bottom corner
    /// </summary>
    /// <returns></returns>
    public virtual int2 MapRoomConvertedOrigin()
    {
        return (MapLocation * new int2(MapSizeX, MapSizeY));
    }
    /// <summary>
    /// Returns 0,0 location for room bottom corner
    /// </summary>
    /// <returns></returns>
    public virtual int2 RoomConvertedOrigin()
    {
        return (RoomConvertedCenter() - new int2((RoomSizeX / 2), (RoomSizeY / 2)));
    }
    /// <summary>
    /// get the center of the room location within the map
    /// </summary>
    /// <returns></returns>
    public virtual int2 RoomConvertedCenter()
    {
        return (MapLocation * new int2(MapSizeX, MapSizeY)) + new int2((MapSizeX / 2), (MapSizeY / 2));
    }
    #endregion
}
