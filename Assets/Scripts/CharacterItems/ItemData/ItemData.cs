using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/Base")]
public class ItemData : ScriptableObject
{
    [Header("Item Data")]
    /// <summary>
    /// Name
    /// </summary>
    public string ItemName;
    /// <summary>
    /// Cost to have this item in use
    /// ie drain on the battery
    /// </summary>
    [SerializeField]
    protected float upKeepCost;
    public float UpKeepCost{ get { return upKeepCost; } }
    /// <summary>
    /// Cost to use the Item
    /// </summary>
    [SerializeField]
    protected float useCost;
    public float UseCost { get { return useCost; } }
}
