using UnityEngine;

/// <summary>
/// this represents a single item
/// </summary>
public class Item : MonoBehaviour
{
    [Header("Item")]
    /// <summary>
    /// Data for this class
    /// </summary>
    public ItemData Data;

    private void Awake()
    {
        PerformAwake();
    }

    protected virtual void PerformAwake()
    {
        CreateClassData();
    }

    private void Start()
    {
        PerformStart();
    }

    protected virtual void PerformStart(){}

    /// <summary>
    /// Create Class data needed for this class
    /// </summary>
    protected virtual void CreateClassData() { }

    /// <summary>
    /// Get the up keep cost for this item
    /// </summary>
    /// <returns></returns>
    public virtual float UpKeepCost() { return Data.UpKeepCost; }

    /// <summary>
    /// Get the Use Cost for this item
    /// </summary>
    /// <returns></returns>
    public virtual float UseCost() { return Data.UseCost; }
}
