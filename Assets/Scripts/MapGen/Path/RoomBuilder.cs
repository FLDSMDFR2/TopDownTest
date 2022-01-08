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
    public virtual void BuildRooms(Transform DestParentTransform, Dictionary<Vector2Int, Room> rooms)
    {
        int roomId = 1;

        // loop over all rooms and build them into the scene
        foreach (var key in rooms.Keys)
        {
            var room = new GameObject("Room " + roomId++);
            room.transform.parent = DestParentTransform;

            var floor = new GameObject("Floor " + room.name);
            floor.layer = (int) Layers.Environment;
            floor.transform.parent = room.transform;
            floor.AddComponent<MeshFilter>();
            var renderer = floor.AddComponent<MeshRenderer>();
            renderer.material = rooms[key].Config.Floor.GetComponentInChildren<Renderer>().sharedMaterial;

            var thevoid = new GameObject("Void " + room.name);
            thevoid.layer = (int) Layers.Environment;
            thevoid.transform.parent = room.transform;
            thevoid.AddComponent<MeshFilter>();
            renderer = thevoid.AddComponent<MeshRenderer>();
            renderer.material = rooms[key].Config.Void.GetComponentInChildren<Renderer>().sharedMaterial;

            var walls = new GameObject("Walls " + room.name);
            walls.layer = (int) Layers.Environment;
            walls.transform.parent = room.transform;
            walls.AddComponent<MeshFilter>();
            renderer = walls.AddComponent<MeshRenderer>();
            renderer.material = rooms[key].Config.Wall.GetComponentInChildren<Renderer>().sharedMaterial;

            // loop over the room locations
            foreach (var locationKey in rooms[key].RoomLocations.Keys)
            {
                placeVoid(key, locationKey, rooms, thevoid);

                placeFloor(key, locationKey, rooms, floor);

                buildWalls(key, locationKey, rooms, walls);

                //buildEnemySpawn(key, locationKey, rooms, room);

                //last thing  we should  do
                PlayerSpawn(key, locationKey, rooms, room);
            }

            var combiner = thevoid.AddComponent<MeshCombiner>();
            combiner.CombineMeshes(true,false);

            combiner = floor.AddComponent<MeshCombiner>();
            combiner.CombineMeshes();

            combiner = walls.AddComponent<MeshCombiner>();
            combiner.CombineMeshes();

            room.isStatic = true;
        }
    }

    protected virtual void placeVoid(Vector2Int key, Vector2Int locationKey, Dictionary<Vector2Int, Room> rooms, GameObject room)
    {
        if (rooms[key].RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.None)
        {
            var thevoid = Instantiate(rooms[key].Config.Void,
                new Vector3(locationKey.x + ObjectSpawnLocation(rooms[key].Config.Void).x,
                ObjectSpawnLocation(rooms[key].Config.Void).y,
                locationKey.y + ObjectSpawnLocation(rooms[key].Config.Void).z), Quaternion.identity, room.transform);
        }
    }

    protected virtual void placeFloor(Vector2Int key, Vector2Int locationKey, Dictionary<Vector2Int, Room> rooms, GameObject room)
    {
        // add wall if needed
        if (rooms[key].RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
            rooms[key].RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door)
        {
            var floor = Instantiate(rooms[key].Config.Floor,
                new Vector3(locationKey.x + ObjectSpawnLocation(rooms[key].Config.Floor).x,
                0f,
                locationKey.y + ObjectSpawnLocation(rooms[key].Config.Floor).z), Quaternion.identity, room.transform);
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
           var p = Instantiate(PlayerPrefab, new Vector3(locationKey.x + ObjectSpawnLocation(PlayerPrefab).x, PlayerPrefab.transform.localScale.y, locationKey.y + ObjectSpawnLocation(PlayerPrefab).z), Quaternion.identity);
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
