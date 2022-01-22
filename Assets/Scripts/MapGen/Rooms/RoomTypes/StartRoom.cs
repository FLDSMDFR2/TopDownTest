using Unity.Mathematics;
using UnityEngine;

public class StartRoom : Room
{
    /// <summary>
    /// Generate data for room
    /// </summary>
    protected override void GenerateRoomByType()
    {
        base.GenerateRoomByType();

        GenerateStartRoomType();
    }

    #region Generate Start Room details
    protected virtual void GenerateStartRoomType()
    {
        var startLocFound = false;
        while (!startLocFound)
        {
            // find random location  and check if we can set as spawn
            var location = new int2(RandomGenerator.SeededRange(Data.RoomConvertedOrigin().x, Data.RoomConvertedOrigin().x + Data.RoomSizeX), 
                RandomGenerator.SeededRange(Data.RoomConvertedOrigin().y, Data.RoomConvertedOrigin().y + Data.RoomSizeY));

            if (Data.RoomLocations.ContainsKey(location) &&
                Data.RoomLocations[location].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor &&
              Data.RoomLocations[location].LocationType == RoomLocationTypes.Empty)
            {
                // if the location is a floor and is empty set as spwan location
                Data.RoomLocations[location].LocationType = RoomLocationTypes.PlayerStartSpawn;
                startLocFound = true;
            }
        }
    }
    #endregion


    #region Build Start Room
    /// <summary>
    /// Add room specific elements
    /// </summary>
    /// <param name="locationKey"></param>
    protected override void PlaceRoomLocationElements(int2 locationKey)
    {
        base.PlaceRoomLocationElements(locationKey);

        // spawn the player
        if (Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor &&
            Data.RoomLocations[locationKey].LocationType == RoomLocationTypes.PlayerStartSpawn)
        {
            var p = Instantiate(Data.PrefabConfig.Player, 
                new Vector3(locationKey.x + ObjectSpawnLocation(Data.PrefabConfig.Player).x, 
                Data.PrefabConfig.Player.transform.localScale.y, 
                locationKey.y + ObjectSpawnLocation(Data.PrefabConfig.Player).z), 
                Quaternion.identity);
            PlayerManager.AddPlayer(p.GetComponent<BasePlayer>());
        }
    }
    #endregion
}
