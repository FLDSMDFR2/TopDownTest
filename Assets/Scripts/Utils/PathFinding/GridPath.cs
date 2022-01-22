using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class GridPath
 {
    protected MapGenerator roomGenerator;

    public virtual void SetRoomGenerator(MapGenerator rg)
    {
        roomGenerator = rg;
    }

    // check if location is within a room
    public virtual bool InBounds(int2 id)
    {
        return roomGenerator.GetRooms().ContainsKey(roomGenerator.GetRoomKey(id));
    }

    public virtual bool Passable(float3 id)
    {
        return Passable(Round(id));
    }

    // check to see if this location is blocked or not
    public virtual bool Passable(int2 id)
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
    public virtual float Cost(int2 a, int2 b)
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
    public virtual IEnumerable<int2> Neighbors(int2 id)
    {
        foreach (var dir in MapTraversal.NeighborsDirectionsAll)
        {
            int2 next = new int2(id.x + dir.x, id.y + dir.y);
            if (InBounds(next) && Passable(next))
            {
                yield return next;
            }
        }
    }

    public int2 Round(float3 loc)
    {

        int2 retval = new int2();

        if (loc.x % 1 >= .5)
        {
            retval.x = ((int)loc.x) + 1;
        }
        else
        {
            retval.x = ((int)loc.x);
        }

        if (loc.z % 1 >= .5)
        {
            retval.y = ((int)loc.z) + 1;
        }
        else
        {
            retval.y = ((int)loc.z);
        }

        return retval;
    }
}
