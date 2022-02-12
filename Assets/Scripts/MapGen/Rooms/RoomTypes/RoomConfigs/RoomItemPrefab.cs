using System;
using UnityEngine;

public enum RoomItemType
{
    None,
    EnemySpwan,
    Charger
}

/// <summary>
/// Type mapping to prefab for Room Item
/// </summary>
[Serializable]
public class RoomItemPrefab
{
    [SerializeField]
    public RoomItemType Type;
    [SerializeField]
    public GameObject Prefab;
}
