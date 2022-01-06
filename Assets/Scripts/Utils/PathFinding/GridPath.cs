using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridPath
 {

    // DIRS is directions
    // I added diagonals to this but noticed it can create problems--
    // like the path will go through obstacles that are diagonal from each other.
    protected static readonly Vector2Int[] DIRS = new[] {
        new Vector2Int(1, 0), // to right of tile
        new Vector2Int(0, -1), // below tile
        new Vector2Int(-1, 0), // to left of tile
        new Vector2Int(0, 1), // above tile
        new Vector2Int(1, 1), // diagonal top right
        new Vector2Int(-1, 1), // diagonal top left
        new Vector2Int(1, -1), // diagonal bottom right
        new Vector2Int(-1, -1) // diagonal bottom left
     };

    protected MapGenerator roomGenerator;

    public virtual void SetRoomGenerator(MapGenerator rg)
    {
        roomGenerator = rg;
    }


    // check if location is within a room
    public virtual bool InBounds(Vector2Int id)
    {
        return roomGenerator.GetRooms().ContainsKey(roomGenerator.GetRoomKey(id));
    }

    // check to see if this location is blocked or not
    public virtual bool Passable(Vector2Int id)
    {       
        if (roomGenerator.GetRooms().ContainsKey(roomGenerator.GetRoomKey(id)))
        {
            var room = roomGenerator.GetRooms()[(roomGenerator.GetRoomKey(id))];
            if (room.RoomLocations.ContainsKey(id))
            {
                return room.RoomLocations[id].LocationTraversalType != RoomLocationTraversalTypes.Blocked;
            }

            return false;
        }

        return false;
    }

    // If the heuristic = 2f, the movement is diagonal
    public virtual float Cost(Vector2Int a, Vector2Int b)
    {

        if (roomGenerator.GetRooms().ContainsKey(roomGenerator.GetRoomKey(b)))
        {
            var room = roomGenerator.GetRooms()[(roomGenerator.GetRoomKey(b))];
            if (room.RoomLocations.ContainsKey(b))
            {
                //if (PathFinding.Heuristic(a, b) == 2f)
                //{
                //    return (float)(int)room.RoomLocations[b].LocationTraversalCost * Mathf.Sqrt(2f);
                //}
                return (float)(int)room.RoomLocations[b].LocationTraversalCost;
            }

            return (float)System.Int32.MaxValue;
        }

        return (float)System.Int32.MaxValue;
    }

    // Check the tiles that are next to, above, below, or diagonal to
    // this tile, and return them if they're within the game bounds and passable
    public virtual IEnumerable<Vector2Int> Neighbors(Vector2Int id)
    {
        foreach (var dir in DIRS)
        {
            Vector2Int next = new Vector2Int(id.x + dir.x, id.y + dir.y);
            if (InBounds(next) && Passable(next))
            {
                yield return next;
            }
        }
    }
}
