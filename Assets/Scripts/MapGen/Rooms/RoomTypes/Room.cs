﻿using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    /// <summary>
    /// Data for this room
    /// </summary>
    [SerializeField]
    public RoomData Data;
    /// <summary>
    /// Referance to the game object to store and build Floor component of the room
    /// </summary>
    protected GameObject FloorGO;
    /// <summary>
    /// Referance to the game object to store and build wall component of the room
    /// </summary>
    protected GameObject WallsGO;
    /// <summary>
    /// Referance to the game object to store and build void component of the room
    /// </summary>
    protected GameObject VoidGO;

    public virtual void GenerateRoomLayout()
    {
        // generate room location data
        GenerateRoomLocations();

        //TODO : maybe this is pulled out into a new class? not sure i want to maintain to classes for each room type
        //visually build out the room based on the data
        BuildRoom();
    }

    #region Room Generation Locations
    /// <summary>
    /// Generate a room from room data supplied
    /// </summary>
    protected virtual void GenerateRoomLocations()
    {
        Data.RoomLocations = new Dictionary<Vector2Int, RoomLocationData>();

        //set origin to the map origin for the room
        var origin = Data.MapRoomConvertedOrigin();

        // loop over the whole room map set all location to None by default, also check for hallways as this will be outside the room but within the map
        for (int i = origin.x; i < origin.x + Data.MapSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + Data.MapSizeY; j++)
            {
                var key = new Vector2Int(i, j);

                if (GenerateHallway(key))
                {
                    // if this is a hallway mark it as floor
                    Data.RoomLocations.Add(key, new RoomLocationData(key, RoomLocationEnvironmentTypes.Floor, RoomLocationTypes.Empty));
                }
                else
                {
                    // add all locations as None
                    Data.RoomLocations.Add(key, new RoomLocationData(key, RoomLocationEnvironmentTypes.None, RoomLocationTypes.Empty));
                }
            }
        }

        // update origin to the rooms origin
        origin = Data.RoomConvertedOrigin();

        // loop over the room locations
        for (int i = origin.x; i < origin.x + Data.RoomSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + Data.RoomSizeY; j++)
            {
                var key = new Vector2Int(i, j);

                // Check to place walls and doors if we dont place a wall or door then place a floor
                if (!GenerateDoors(key, origin.x, origin.y) && Data.RoomLocations.ContainsKey(key))
                {
                    // update location to floor as its should be in the room and not a wall or door
                    Data.RoomLocations[key].EnvironmentLocationType = RoomLocationEnvironmentTypes.Floor;
                }
            }
        }

        origin = Data.MapRoomConvertedOrigin();

        // loop over the whole room map set all location to None by default, also check for hallways as this will be outside the room but within the map
        for (int i = origin.x; i < origin.x + Data.MapSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + Data.MapSizeY; j++)
            {
                var key = new Vector2Int(i, j);

                if (GenerateWall(key))
                {
                    // if this is a hallway mark it as floor
                    Data.RoomLocations[key].EnvironmentLocationType = RoomLocationEnvironmentTypes.Wall;
                }
            }
        }

        // add room type specific details
        GenerateRoomByType();
    }

    #region Generate Room Helpers
    /// <summary>
    /// Generate door locations
    /// </summary>
    /// <param name="key">location in room to check if we should place a door</param>
    /// <param name="startX">bounds x to check against</param>
    /// <param name="StartY">bounds y to check against</param>
    /// <returns></returns>
    protected virtual bool GenerateDoors(Vector2Int key, int startX, int StartY)
    {
        //  check if we are the outer edge of the room
        if (key.x == startX || key.y == StartY || key.x == (startX + Data.RoomSizeX) - 1 || key.y == (StartY + Data.RoomSizeY) - 1)
        {
            // get the side we are checking for door
            DoorSide side = DoorSide.None;
            if (key.x == startX)
                side = DoorSide.Left;
            else if (key.y == StartY)
                side = DoorSide.Down;
            else if (key.x == (startX + Data.RoomSizeX) - 1)
                side = DoorSide.Right;
            else if (key.y == (StartY + Data.RoomSizeY) - 1)
                side = DoorSide.Up;

            // if the location is a door then place it
            if (GenerateDoor(key, side) && Data.RoomLocations.ContainsKey(key))
            {
                // update this location to be a door
                Data.RoomLocations[key].EnvironmentLocationType = RoomLocationEnvironmentTypes.Door;
            }
            return true;
        }

        return false;
    }
    /// <summary>
    /// Return if the location provided is a hall location
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    protected virtual bool GenerateHallway(Vector2Int location)
    {
        var origin = Data.RoomConvertedOrigin();

        // check if the location is outside the room
        if (location.x < origin.x || location.y < origin.y || location.x > (origin.x + Data.RoomSizeX) - 1 || location.y > (origin.y + Data.RoomSizeY) - 1)
        {
            // location is outside the room but on the room map
            if (location.x < origin.x)
                return CheckDoorForHallway(location, DoorSide.Left);
            if (location.x > (origin.x + Data.RoomSizeX) - 1)
                return CheckDoorForHallway(location, DoorSide.Right);
            if (location.y < origin.y)
                return CheckDoorForHallway(location, DoorSide.Down);
            if (location.y > (origin.y + Data.RoomSizeY) - 1)
                return CheckDoorForHallway(location, DoorSide.Up);
        }

        return false;
    }

    protected virtual bool CheckDoorForHallway(Vector2Int location, DoorSide side)
    {
        foreach (var d in Data.Doors)
        {
            if (d.Side == side)
            {
                if (side == DoorSide.Left || side == DoorSide.Right)
                {
                    // if the location is in the window of the door
                    if (location.y <= d.ConnectionPoint.y + d.Size && location.y >= d.ConnectionPoint.y - d.Size)
                        return true;
                }
                else
                {
                    // if the location is in the window of the door
                    if (location.x <= d.ConnectionPoint.x + d.Size && location.x >= d.ConnectionPoint.x - d.Size)
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// If location supplied is a door location
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    protected virtual bool GenerateDoor(Vector2Int location, DoorSide side)
    {
        foreach (var d in Data.Doors)
        {
            if (d.Side == side)
            {
                if (side == DoorSide.Left || side == DoorSide.Right)
                {
                    // if the location is in the window of the door
                    if (location.y <= d.ConnectionPoint.y + d.Size && location.y >= d.ConnectionPoint.y - d.Size)
                        return true;
                }
                else
                {
                    // if the location is in the window of the door
                    if (location.x <= d.ConnectionPoint.x + d.Size && location.x >= d.ConnectionPoint.x - d.Size)
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// If location supplied should have a wall
    /// </summary>
    /// <param name="location">location to check</param>
    /// <returns></returns>
    protected virtual bool GenerateWall(Vector2Int location)
    {
        // if we are outside the room
        if (Data.RoomLocations[location].EnvironmentLocationType == RoomLocationEnvironmentTypes.None)
        {
            //check if we are next to a floor or door if so then we need to be a wall

            //right
            var checkLoc = location + new Vector2Int(1, 0);
            if (Data.RoomLocations.ContainsKey(checkLoc) &&
                (Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;

            //left
            checkLoc = location + new Vector2Int(-1, 0);
            if (Data.RoomLocations.ContainsKey(checkLoc) &&
               (Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;

            //up
            checkLoc = location + new Vector2Int(0, 1);
            if (Data.RoomLocations.ContainsKey(checkLoc) &&
                (Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;

            //down
            checkLoc = location + new Vector2Int(0, -1);
            if (Data.RoomLocations.ContainsKey(checkLoc) &&
                (Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// Add Room type details
    /// </summary>
    protected virtual void GenerateRoomByType()
    {
        // add room obstacles .... most rooms should have these
        GenerateRoomObstacles();
    }

    #region Room Obstacles
    /// <summary>
    /// Add random obstacles to the room
    /// </summary>
    protected virtual void GenerateRoomObstacles()
    {
        // TODO: ADD
    }
    #endregion

    #endregion

    #region BuildRoom
    /// <summary>
    /// Builds the room
    /// </summary>
    protected virtual void BuildRoom()
    {
        BuildEnvironmentHolders();

        // loop over the room locations
        foreach (var locationKey in Data.RoomLocations.Keys)
        {
            PlaceRoomLocationElements(locationKey);
        }

        CombineRoomMeshes();

        gameObject.isStatic = true;
    }
    /// <summary>
    /// Create Holder GO for like elements so we can combine their meshes
    /// </summary>
    protected virtual void BuildEnvironmentHolders()
    {
        // create floor
        FloorGO = CreateEnvironmentHolders(FloorGO, Data.PrefabConfig.Floor, "Floor");

        // create void
        VoidGO = CreateEnvironmentHolders(VoidGO, Data.PrefabConfig.Void, "Void");

        // create walls
        WallsGO = CreateEnvironmentHolders(WallsGO, Data.PrefabConfig.Wall, "Walls");
    }

    /// <summary>
    /// Setup holder GO
    /// </summary>
    /// <param name="holder">Holder object</param>
    /// <param name="environment">type of object we are going to create</param>
    /// <param name="Name">name for holder</param>
    protected virtual GameObject CreateEnvironmentHolders(GameObject holder, GameObject environment, string Name)
    {
        holder = new GameObject(Name + " " + gameObject.name);
        holder.layer = (int)Layers.Environment;
        holder.transform.parent = gameObject.transform;
        holder.AddComponent<MeshFilter>();
        var renderer = holder.AddComponent<MeshRenderer>();
        renderer.material = environment.GetComponentInChildren<Renderer>().sharedMaterial;

        return holder;
    }

    /// <summary>
    /// Instantiate the location elements by RoomLocationEnvironmentTypes
    /// </summary>
    /// <param name="locationKey">room location key</param>
    protected virtual void PlaceRoomLocationElements(Vector2Int locationKey)
    {
        //void
        if (Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.None)
        {
            var thevoid = Instantiate(Data.PrefabConfig.Void,
                new Vector3(locationKey.x + ObjectSpawnLocation(Data.PrefabConfig.Void).x,
                ObjectSpawnLocation(Data.PrefabConfig.Void).y,
                locationKey.y + ObjectSpawnLocation(Data.PrefabConfig.Void).z), Quaternion.identity, VoidGO.transform);
        }
        //floor
        if (Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
            Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door)
        {
            var floor = Instantiate(Data.PrefabConfig.Floor,
                new Vector3(locationKey.x + ObjectSpawnLocation(Data.PrefabConfig.Floor).x,
                0f,
                locationKey.y + ObjectSpawnLocation(Data.PrefabConfig.Floor).z), Quaternion.identity, FloorGO.transform);
        }
        //wall
        if (Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
        {
            var wall = Instantiate(Data.PrefabConfig.Wall,
                new Vector3(locationKey.x + ObjectSpawnLocation(Data.PrefabConfig.Wall).x,
                ObjectSpawnLocation(Data.PrefabConfig.Wall).y,
                locationKey.y + ObjectSpawnLocation(Data.PrefabConfig.Wall).z), Quaternion.identity, WallsGO.transform);
        }
    }
    /// <summary>
    /// Combine meshes for room
    /// </summary>
    protected virtual void CombineRoomMeshes()
    {
        var combiner = VoidGO.AddComponent<MeshCombiner>();
        combiner.CombineMeshes(true, false);

        combiner = FloorGO.AddComponent<MeshCombiner>();
        combiner.CombineMeshes();

        combiner = WallsGO.AddComponent<MeshCombiner>();
        combiner.CombineMeshes();
    }
    #endregion

    #region Helpers
    protected virtual Vector3 ObjectSpawnLocation(GameObject obj)
    {
        return new Vector3(0, obj.transform.localScale.y / 2, 0);
    }
    #endregion
}
