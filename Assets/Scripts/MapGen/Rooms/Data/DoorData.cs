using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum DoorSide
{
    None,
    Up,
    Down,
    Left,
    Right
}

/// <summary>
/// Connection between rooms
/// </summary>
[Serializable]
public class DoorData
{
    /// <summary>
    /// center of the door
    /// </summary>
    [SerializeField]
    public int2 ConnectionPoint;

    /// <summary>
    ///  what side of the room the door is on
    /// </summary>
    [SerializeField]
    public DoorSide Side;

    /// <summary>
    /// Rooms connected by this door
    /// </summary>
    [SerializeField]
    public List<RoomData> ConnectedRooms = new List<RoomData>();

    /// <summary>
    /// Size of the door
    /// </summary>
    [SerializeField]
    public int Size;

    public DoorData(int2 connectionPoint, DoorSide side, List<RoomData> rooms, int size)
    {
        ConnectionPoint = connectionPoint;
        Side = side;
        ConnectedRooms = rooms;
        Size = size;
    }
}
