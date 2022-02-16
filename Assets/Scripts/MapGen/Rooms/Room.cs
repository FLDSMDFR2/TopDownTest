using System.Collections.Generic;
using Unity.Mathematics;
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
        Data.RoomLocations = new Dictionary<int2, RoomLocationData>();

        //set origin to the map origin for the room
        var origin = Data.MapRoomConvertedOrigin();

        // loop over the whole room map set all location to None by default, 
        // also check for hallways as this will be outside the room but within the map
        for (int i = origin.x; i < origin.x + Data.MapSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + Data.MapSizeY; j++)
            {
                var key = new int2(i, j);

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

        // loop over all locations within the room
        for (int i = origin.x; i < origin.x + Data.RoomSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + Data.RoomSizeY; j++)
            {
                var key = new int2(i, j);

                // Check to place walls and doors if we dont place a wall or door then place a floor
                if (!GenerateDoors(key, origin.x, origin.y) && Data.RoomLocations.ContainsKey(key))
                {
                    // update location to floor as its should be in the room and not a wall or door
                    Data.RoomLocations[key].EnvironmentLocationType = RoomLocationEnvironmentTypes.Floor;
                }
            }
        }

        origin = Data.MapRoomConvertedOrigin();

        // After setting floors and the void loop the whole map location again to check for wall locations
        // any location that has a floor touching the void
        for (int i = origin.x; i < origin.x + Data.MapSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + Data.MapSizeY; j++)
            {
                var key = new int2(i, j);

                if (GenerateWall(key))
                {
                    // if this is a hallway mark it as floor
                    Data.RoomLocations[key].LocationType = RoomLocationTypes.Filled;
                    Data.RoomLocations[key].EnvironmentLocationType = RoomLocationEnvironmentTypes.Wall;
                }
            }
        }

        // add room type specific details
        GenerateRoomByType();

        //try and add items to this room
        GenerateItems();
    }

    #region Generate Room Helpers
    /// <summary>
    /// Generate door locations
    /// </summary>
    /// <param name="key">location in room to check if we should place a door</param>
    /// <param name="startX">bounds x to check against</param>
    /// <param name="StartY">bounds y to check against</param>
    /// <returns></returns>
    protected virtual bool GenerateDoors(int2 key, int startX, int StartY)
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
    protected virtual bool GenerateHallway(int2 location)
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

    protected virtual bool CheckDoorForHallway(int2 location, DoorSide side)
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
    protected virtual bool GenerateDoor(int2 location, DoorSide side)
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
    protected virtual bool GenerateWall(int2 location)
    {
        // if we are outside the room
        if (Data.RoomLocations[location].EnvironmentLocationType == RoomLocationEnvironmentTypes.None)
        {
            //check if we are next to a floor or door if so then we need to be a wall

            //right        
            var checkLoc = location + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Right];
            if (Data.RoomLocations.ContainsKey(checkLoc) &&
                (Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;

            //left
            checkLoc = location + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Left];
            if (Data.RoomLocations.ContainsKey(checkLoc) &&
               (Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;

            //up
            checkLoc = location + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Up];
            if (Data.RoomLocations.ContainsKey(checkLoc) &&
                (Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
                Data.RoomLocations[checkLoc].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door))
                return true;

            //down
            checkLoc = location + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Down];
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

    /// <summary>
    /// Add items to this room
    /// </summary>
    protected virtual void GenerateItems()
    {
        // if this room has not items then just skip this
        if (Data.RoomItems == null || Data.RoomItems.Count <= 0) return;

        // loop over all room locations trying to place items
        var origin = Data.RoomConvertedOrigin();

        // loop over all locations within the room
        for (int i = origin.x; i < origin.x + Data.RoomSizeX; i++)
        {
            for (int j = origin.y; j < origin.y + Data.RoomSizeY; j++)
            {
                var key = new int2(i, j);

                // if this location is the void or a wall we cant place an item here so skip
                if (Data.RoomLocations[key].EnvironmentLocationType == RoomLocationEnvironmentTypes.None ||
                    Data.RoomLocations[key].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
                    continue;

                    // check each item to see if we can place it at this location within the room
                    foreach (var item in Data.RoomItems)
                {
                    if (item.CanPlaceItem(this, key))
                    {
                        Data.RoomLocations[key].Item = item;
                        Data.RoomLocations[key].LocationType = RoomLocationTypes.Item;

                        // if we have placed an item at this location then we cant place another so break
                        break;
                    }
                }
            }
        }
    }

    #region Room Obstacles
    /// <summary>
    /// Add random obstacles to the room
    /// </summary>
    protected virtual void GenerateRoomObstacles()
    {
        foreach (var locKey in Data.RoomLocations.Keys)
        {
            // if the location doesnt have a wall and is empty this might be an obstacle in the room
            if (Data.RoomLocations[locKey].EnvironmentLocationType != RoomLocationEnvironmentTypes.Wall &&
                Data.RoomLocations[locKey].LocationType == RoomLocationTypes.Empty)
            {
                //check ajasent location for walls this would be increaded odds of a wall here then
                var wallChange = RandomGenerator.SeededRange(1, 100);
                var percentChange = 1;
                if (HasTypeAdjacent(locKey, RoomLocationEnvironmentTypes.Wall)) percentChange = 55;

                if (wallChange < percentChange && ObstacleCheck(locKey))
                {
                    Data.RoomLocations[locKey].LocationType = RoomLocationTypes.Filled;
                    Data.RoomLocations[locKey].EnvironmentLocationType = RoomLocationEnvironmentTypes.Wall;
                }
            }
        }
    }

    /// <summary>
    /// Perform check if Obstacle can / should be placed
    /// </summary>
    /// <returns></returns>
    protected virtual bool ObstacleCheck(int2 loc)
    {
        // check if any walls are at a diagnal if so then dont place 

        //Up Left
        var checkDiag = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.UpLeft];
        if (Data.RoomLocations.ContainsKey(checkDiag) && Data.RoomLocations[checkDiag].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
        {
            // if we have a wall diagnal then check the two orthognial directs for walls if we dont have at least one then we dont want to place this wall
            var org1 = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Up];
            var org2 = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Left];
            if ((Data.RoomLocations.ContainsKey(org1) && Data.RoomLocations[org1].EnvironmentLocationType != RoomLocationEnvironmentTypes.Wall) &&
                (Data.RoomLocations.ContainsKey(org2) && Data.RoomLocations[org2].EnvironmentLocationType != RoomLocationEnvironmentTypes.Wall))
            {
                return false;
            }
        }

        //Up right
        checkDiag = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.UpRight];
        if (Data.RoomLocations.ContainsKey(checkDiag) && Data.RoomLocations[checkDiag].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
        {
            // if we have a wall diagnal then check the two orthognial directs for walls if we dont have at least one then we dont want to place this wall
            var org1 = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Up];
            var org2 = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Right];
            if ((Data.RoomLocations.ContainsKey(org1) && Data.RoomLocations[org1].EnvironmentLocationType != RoomLocationEnvironmentTypes.Wall) &&
                (Data.RoomLocations.ContainsKey(org2) && Data.RoomLocations[org2].EnvironmentLocationType != RoomLocationEnvironmentTypes.Wall))
            {
                return false;
            }
        }

        //down right
        checkDiag = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.DownRight];
        if (Data.RoomLocations.ContainsKey(checkDiag) && Data.RoomLocations[checkDiag].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
        {
            // if we have a wall diagnal then check the two orthognial directs for walls if we dont have at least one then we dont want to place this wall
            var org1 = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Down];
            var org2 = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Right];
            if ((Data.RoomLocations.ContainsKey(org1) && Data.RoomLocations[org1].EnvironmentLocationType != RoomLocationEnvironmentTypes.Wall) &&
                (Data.RoomLocations.ContainsKey(org2) && Data.RoomLocations[org2].EnvironmentLocationType != RoomLocationEnvironmentTypes.Wall))
            {
                return false;
            }
        }

        //down left
        checkDiag = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.DownLeft];
        if (Data.RoomLocations.ContainsKey(checkDiag) && Data.RoomLocations[checkDiag].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
        {
            // if we have a wall diagnal then check the two orthognial directs for walls if we dont have at least one then we dont want to place this wall
            var org1 = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Down];
            var org2 = loc + MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Left];
            if ((Data.RoomLocations.ContainsKey(org1) && Data.RoomLocations[org1].EnvironmentLocationType != RoomLocationEnvironmentTypes.Wall) &&
                (Data.RoomLocations.ContainsKey(org2) && Data.RoomLocations[org2].EnvironmentLocationType != RoomLocationEnvironmentTypes.Wall))
            {
                return false;
            }
        }

        return true;
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
            PlaceRoomLocationEnvironmentElements(locationKey);
            PlaceRoomLocationItems(locationKey);
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
    protected virtual void PlaceRoomLocationEnvironmentElements(int2 locationKey)
    {
        //void
        if (Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.None)
        {
            var prefab = Data.PrefabConfig.Void;
            var thevoid = Instantiate(prefab,
                new Vector3(locationKey.x + ObjectSpawnLocation(prefab).x,
                ObjectSpawnLocation(prefab).y,
                locationKey.y + ObjectSpawnLocation(prefab).z), Quaternion.identity, VoidGO.transform);
        }
        //floor
        if (Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Floor ||
            Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Door)
        {
            var prefab = Data.PrefabConfig.Floor;
            var floor = Instantiate(prefab,
                new Vector3(locationKey.x + ObjectSpawnLocation(prefab).x,
                0f,
                locationKey.y + ObjectSpawnLocation(prefab).z), Quaternion.identity, FloorGO.transform);
        }
        //wall
        if (Data.RoomLocations[locationKey].EnvironmentLocationType == RoomLocationEnvironmentTypes.Wall)
        {
            var prefab = Data.PrefabConfig.Wall;
            var wall = Instantiate(prefab,
                new Vector3(locationKey.x + ObjectSpawnLocation(prefab).x,
                ObjectSpawnLocation(prefab).y,
                locationKey.y + ObjectSpawnLocation(prefab).z), Quaternion.identity, WallsGO.transform);
        }
    }

    /// <summary>
    /// Place items for this location
    /// </summary>
    /// <param name="locationKey"></param>
    protected virtual void PlaceRoomLocationItems(int2 locationKey)
    {
        // if an item should be here and the item is not null place it
        if (Data.RoomLocations[locationKey].LocationType == RoomLocationTypes.Item && Data.RoomLocations[locationKey].Item != null)
        {
            //get the prefab for this item type
            var prefab = Data.PrefabConfig.GetRoomItemPrefab(Data.RoomLocations[locationKey].Item.Type);
            // if we dont have prefab configured for this item return and dont place it
            if (prefab == null) return;

            //create the object
            var item = Instantiate(prefab,
                new Vector3(locationKey.x + ObjectSpawnLocation(prefab).x,
                ObjectSpawnLocation(prefab).y,
                locationKey.y + ObjectSpawnLocation(prefab).z), Quaternion.identity, gameObject.transform);

            var itemObj = item.GetComponent<RoomItemObject>();
            if (itemObj != null)
            {
                itemObj.Item = Data.RoomLocations[locationKey].Item;
            }
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
    public virtual bool HasTypeAdjacent(int2 locKey, RoomLocationEnvironmentTypes type)
    {
        foreach (var dir in MapTraversal.NeighborsDirectionsAll)
        {
            var checkKey = locKey + dir;
            if (Data.RoomLocations.ContainsKey(checkKey) && Data.RoomLocations[checkKey].EnvironmentLocationType == type)
                return true;
        }

        return false;
    }

    protected virtual Vector3 ObjectSpawnLocation(GameObject obj)
    {
        return new Vector3(0, obj.transform.localScale.y / 2, 0);
    }
    #endregion
}
