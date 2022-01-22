using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to set up a test room
/// </summary>
public class RoomGeneratorTester : RoomGenerator
{
    protected override void UpdateRoomsTypes()
    {
        //TODO : MAKE THIS BETTER
        var count = 0;

        var mapRooms = mapGenerator.GetRooms();
        foreach (var key in mapRooms.Keys)
        {
            if (count <= 0)
            {
                //set first room to start room
                mapRooms[key].RoomType = RoomTypes.Test;
                mapRooms[key].PrefabConfig = Config;
                mapRooms[key].Difficulty = RoomDifficulty.Easy;
            }
            else
            {
                mapRooms[key].RoomType = RoomTypes.None;
                mapRooms[key].PrefabConfig = Config;
                mapRooms[key].Difficulty = RoomDifficulty.Easy;
            }
            count++;
        }
    }

    protected override void GenerateRoom(RoomData data, int roomId)
    {
        var go = new GameObject("Room " + roomId++);
        go.transform.parent = destParentTransform;
        Room room;

        switch (data.RoomType)
        {
            case RoomTypes.Test:
                room = go.AddComponent<TestRoom>();
                break;
            default:
                room = go.AddComponent<Room>();
                break;
        }
        if (room != null)
        {
            // add data to the room then add to created list
            room.Data = data;
            rooms.Add(room);
        }
    }
}
