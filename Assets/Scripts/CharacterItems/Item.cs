using UnityEngine;

/// <summary>
/// this represents a single item
/// </summary>
public class Item : MonoBehaviour
{
    [Header("Item")]
    /// <summary>
    /// Cost to have this item in use
    /// ie drain on the battery
    /// </summary>
    [SerializeField]
    protected float upKeepCost;
    /// <summary>
    /// Cost to use the Item
    /// </summary>
    [SerializeField]
    protected float useCost;

    /// <summary>
    /// Get the up keep cost for this item
    /// </summary>
    /// <returns></returns>
    public virtual float UpKeepCost() { return upKeepCost; }

    /// <summary>
    /// Get the Use Cost for this item
    /// </summary>
    /// <returns></returns>
    public virtual float UseCost() { return useCost; }
}
