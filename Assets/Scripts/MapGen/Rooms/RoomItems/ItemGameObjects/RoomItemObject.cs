using UnityEngine;

/// <summary>
/// Used to attach room items to Game Objects
/// </summary>
public class RoomItemObject : MonoBehaviour
{
    /// <summary>
    /// Room Item Class
    /// </summary>
    [Header("Room Item")]
    [SerializeField]
    public RoomItem Item;

    protected virtual void Awake()
    {
        PerformAwakeInit();
        GenerateConfigValues();
    }

    /// <summary>
    /// Perform Awake init
    /// </summary>
    protected virtual void PerformAwakeInit() { }
    /// <summary>
    /// Generate Config Values as needed for class specific setup
    /// </summary>
    protected virtual void GenerateConfigValues() { }

    protected virtual void PerformItemLogic() { }
}
