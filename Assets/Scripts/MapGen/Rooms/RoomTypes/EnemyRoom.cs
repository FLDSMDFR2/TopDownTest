using UnityEngine;

public class EnemyRoom : Room
{
    /// <summary>
    /// Generate data for room
    /// </summary>
    protected override void GenerateRoomByType()
    {
        base.GenerateRoomByType();

        GenerateEnemyRoomType();
    }

    #region Generate Enemy Room details
    protected virtual void GenerateEnemyRoomType()
    {
        var locFound = false;
        while (!locFound)
        {
            // find random location  and check if we can set as spawn
            var location = new Vector2Int(RandomGenerator.SeededRange(Data.RoomConvertedOrigin().x, Data.RoomConvertedOrigin().x + Data.RoomSizeX), 
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
    protected override void PlaceRoomLocationElements(Vector2Int locationKey)
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
