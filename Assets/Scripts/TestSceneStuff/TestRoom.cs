using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Test room to make dev easier... hopefully
/// 1,700 X  1,200 Y 
/// </summary>
public class TestRoom : StartRoom
{
    #region Generate Start Room details
    protected override void GenerateStartRoomType()
    {
        var startLocFound = false;
        while (!startLocFound)
        {
            // find random location  and check if we can set as spawn
            var location = new int2(1700 + 25, 1200 + 5);

            if (Data.RoomLocations.ContainsKey(location) &&
                Data.RoomLocations[location].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor &&
              Data.RoomLocations[location].LocationType == RoomLocationTypes.Empty)
            {
                // if the location is a floor and is empty set as spwan location
                Data.RoomLocations[location].LocationType = RoomLocationTypes.PlayerStartSpawn;
                startLocFound = true;
            }
        }

        GenerateEnemyRoomType();
    }

    protected virtual void GenerateEnemyRoomType()
    {
        var locFound = false;
        while (!locFound)
        {
            // find random location  and check if we can set as spawn
            var location = new int2(RandomGenerator.SeededRange(Data.RoomConvertedOrigin().x, Data.RoomConvertedOrigin().x + Data.RoomSizeX),
                RandomGenerator.SeededRange(Data.RoomConvertedOrigin().y, Data.RoomConvertedOrigin().y + Data.RoomSizeY));

            if (Data.RoomLocations.ContainsKey(location) &&
                Data.RoomLocations[location].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor &&
               Data.RoomLocations[location].LocationType == RoomLocationTypes.Empty)
            {
                // if the location is a floor and is empty set as enemy spwan location
                Data.RoomLocations[location].LocationType = RoomLocationTypes.EnemySpawn;
                locFound = true;
            }
        }
    }
    #endregion

    #region Build Enemy Room
    /// <summary>
    /// Add room specific elements
    /// </summary>
    /// <param name="locationKey"></param>
    protected override void PlaceRoomLocationElements(int2 locationKey)
    {
        base.PlaceRoomLocationElements(locationKey);

        // spawn Enemy
        if (Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor &&
            Data.RoomLocations[locationKey].LocationType == RoomLocationTypes.EnemySpawn)
        {
            // add enemy spawn to this location
            var spawn = gameObject.AddComponent<EnemySpawn>();
            spawn.Location = new Vector3(locationKey.x, 1f, locationKey.y); ;
            spawn.MaxEnemysToSpawn = 5;
            spawn.difficulty = Data.Difficulty;
        }
    }
    #endregion
}
