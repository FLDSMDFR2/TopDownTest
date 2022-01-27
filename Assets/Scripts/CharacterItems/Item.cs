using UnityEngine;

/// <summary>
/// this represents a single item
/// </summary>
public class Item : MonoBehaviour
{
    #region Variables
    [Header("Item")]
    /// <summary>
    /// Data for this class
    /// </summary>
    public ItemData Data;
    #endregion

    #region Init
    /// <summary>
    /// Perform Awake init
    /// </summary>
    private void Awake()
    {
        PerformAwake();
    }

    /// <summary>
    /// Perform class independent init
    /// </summary>
    protected virtual void PerformAwake()
    {
        CreateClassData();
    }
    /// <summary>
    /// Perform start init
    /// </summary>
    private void Start()
    {
        PerformStart();
    }
    /// <summary>
    /// Perform class dependent init
    /// </summary>
    protected virtual void PerformStart() { }

    /// <summary>
    /// Create Class data needed for this class
    /// </summary>
    protected virtual void CreateClassData() { }
    #endregion

    #region Class Logic
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
    #endregion
}
