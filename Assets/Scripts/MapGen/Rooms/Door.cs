using System.Collections.Generic;
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
public class Door
{
    /// <summary>
    /// center of the door
    /// </summary>
    public Vector2Int ConnectionPoint;

    /// <summary>
    ///  what side of the room the door is on
    /// </summary>
    public DoorSide Side;

    /// <summary>
    /// Rooms connected by this door
    /// </summary>
    public List<Room> ConnectedRooms = new List<Room>();

    /// <summary>
    /// Size of the door
    /// </summary>
    public int Size;

    public Door(Vector2Int connectionPoint, DoorSide side, List<Room> rooms, int size)
    {
        ConnectionPoint = connectionPoint;
        Side = side;
        ConnectedRooms = rooms;
        Size = size;
    }
}
