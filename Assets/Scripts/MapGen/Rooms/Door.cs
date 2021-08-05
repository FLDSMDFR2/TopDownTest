using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum DoorSide
{
    Up,
    Down,
    Left,
    Right
}

public class Door
{
    public List<Vector2Int> DoorLocations;

    public DoorSide Side;

    public List<Room> ConnectedRooms = new List<Room>();

    // if  the door belongs to this room
    public bool IsMyDoor;


    public  Door(List<Vector2Int> Locations, DoorSide side, List<Room> rooms, bool isForThisRoom = true)
    {
        DoorLocations = Locations;
        Side = side;
        ConnectedRooms = rooms;
        IsMyDoor = isForThisRoom;
    }
}
