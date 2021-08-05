using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Room Config", menuName = "Rooms/Base")]
public class RoomConfig : ScriptableObject
{
    public GameObject Floor;
    public GameObject Wall;
}
