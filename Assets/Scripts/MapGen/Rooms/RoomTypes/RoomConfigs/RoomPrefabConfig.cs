using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "RoomPrefabConfig", menuName = "Rooms/PrefabConfig/Base")]
public class RoomPrefabConfig : ScriptableObject
{
    public GameObject Player;
    public GameObject Floor;
    public GameObject Wall;
    public GameObject Void;

    //items 
    [SerializeField]
    public List<RoomItemPrefab> RoomItemPrefabs;

    public virtual GameObject GetRoomItemPrefab(RoomItemType type)
    {
        if (RoomItemPrefabs == null || RoomItemPrefabs.Count <= 0) return null;

        foreach (var prefab in RoomItemPrefabs)
        {
            if (prefab.Type == type)
                return prefab.Prefab;
        }

        return null;
    }
}
