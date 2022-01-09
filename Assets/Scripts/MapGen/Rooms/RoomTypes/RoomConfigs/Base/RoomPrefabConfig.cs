using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Room Prefab Config", menuName = "Rooms/Base")]
public class RoomPrefabConfig : ScriptableObject
{
    public GameObject Player;
    public GameObject Floor;
    public GameObject Wall;
    public GameObject Void;
}
