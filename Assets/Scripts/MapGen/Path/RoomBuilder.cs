using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{

    //TODO: REMOVE LATER
    public GameObject PlayerPrefab;
    //TODO REMOVE LATER


    /// <summary>
    /// Loop over all rooms that have been created and build them in the scene
    /// </summary>
    public virtual void BuildRooms(Transform DestParentTransform, Dictionary<Vector2Int, Room> rooms, int roomSizeX, int roomSizeY)
    {
        int roomId = 1;

        // loop over all rooms and build them into the scene
        foreach (var key in rooms.Keys)
        {
            var room = new GameObject("Room " + roomId++);
            room.transform.parent = DestParentTransform;

            // first place the floor for the room
            var floor = Instantiate(rooms[key].Config.Floor, new Vector3((float)(rooms[key].RoomConvertedCenter().x - .5f), .1f, (float)(rooms[key].RoomConvertedCenter().y - .5f)), Quaternion.identity, room.transform);
            floor.transform.localScale = new Vector3(roomSizeX, .1f, roomSizeY);

            // loop over the room locations
            foreach (var locationKey in rooms[key].RoomLocations.Keys)
            {
                //buildWalls(key, locationKey, rooms, room);

                buildEnemySpawn(key, locationKey, rooms, room);

                //last thing  we should  do
                PlayerSpawn(key, locationKey, rooms, room);
            }
        }
    }

    protected virtual void buildWalls(Vector2Int key, Vector2Int locationKey, Dictionary<Vector2Int, Room> rooms, GameObject room)
    {
        // add wall if needed
        if (rooms[key].RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
        {
            var wall = Instantiate(rooms[key].Config.Wall,
                new Vector3(locationKey.x + ObjectSpawnLocation(rooms[key].Config.Wall).x,
                ObjectSpawnLocation(rooms[key].Config.Wall).y,
                locationKey.y + ObjectSpawnLocation(rooms[key].Config.Wall).z), Quaternion.identity, room.transform);

            var loc = wall.AddComponent<RoomLocation>();
            loc.Location = rooms[key].RoomLocations[locationKey].Location;
            loc.EnvironmentLocationType = rooms[key].RoomLocations[locationKey].EnvironmentLocationType;

        }
    }

    protected virtual void buildEnemySpawn(Vector2Int key, Vector2Int locationKey, Dictionary<Vector2Int, Room> rooms, GameObject room)
    {
        // spawn Enemy
        if (rooms[key].RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor &&
            rooms[key].RoomLocations[locationKey].LocationType == RoomLocationTypes.EnemySpawn)
        {
            // add enemy spawn to this location
            var spawn = room.AddComponent<EnemySpawn>();
            spawn.Location = new Vector3(locationKey.x, 1f, locationKey.y); ;
            spawn.MaxEnemysToSpawn = 5;
            spawn.difficulty = rooms[key].Difficulty;
        }
    }

    protected virtual void PlayerSpawn(Vector2Int key, Vector2Int locationKey, Dictionary<Vector2Int, Room> rooms, GameObject room)
    {
        // spawn the player
        if (rooms[key].RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor &&
            rooms[key].RoomLocations[locationKey].LocationType == RoomLocationTypes.PlayerStartSpawn)
        {
            var p = Instantiate(PlayerPrefab, new Vector3(locationKey.x + ObjectSpawnLocation(PlayerPrefab).x, ObjectSpawnLocation(PlayerPrefab).y, locationKey.y + ObjectSpawnLocation(PlayerPrefab).z), Quaternion.identity);
            PlayerManager.AddPlayer(p.GetComponent<BasePlayer>());
        }
    }


    #region Helpers
    protected virtual Vector3 ObjectSpawnLocation(GameObject obj)
    {
        return new Vector3(0, obj.transform.localScale.y / 2, 0);
    }
    #endregion
}
