using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class RoomItemEnemySpawn : RoomItem
{
    protected int distanceFromWalls = 5;

    #region Config
    protected override void ConfigureSelf()
    {
        Type = RoomItemType.EnemySpwan;
        maxItemUsesInRoom = 1;
    }
    #endregion

    #region Place Location Checks
    protected override bool CheckLocation(Room room, int2 locationKey)
    {
        // check that this location is x Distance from walls in all directions
        foreach (var dir in MapTraversal.NeighborsDirectionsAll)
        {
            var checkLoc = locationKey;
            for (int i = 0; i <= distanceFromWalls; i++)
            {
                checkLoc += dir;
                if (room.Data.RoomLocations.ContainsKey(checkLoc) && room.Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
                    return false; // location is to close to a wall
                else if (!room.Data.RoomLocations.ContainsKey(checkLoc))
                    return false; // location is to close to edge of room
            }
        }

        return true;
    }
    #endregion
}
