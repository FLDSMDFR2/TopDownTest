using UnityEngine;

public enum GameDifficulty
{
    Easy,
    Medium,
    Hard
}

[RequireComponent(typeof(MapGenerator))]
[RequireComponent(typeof(RoomGenerator))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    /// <summary>
    /// Game Difficulty Selected
    /// </summary>
    [SerializeField]
    protected GameDifficulty Difficulty;

    /// <summary>
    /// Map Generator
    /// </summary>
    protected MapGenerator mapGenerator;
    /// <summary>
    /// Room Generator
    /// </summary>
    protected RoomGenerator roomGenerator;
    /// <summary>
    /// Transform to place built GO into
    /// </summary>
    [SerializeField]
    protected Transform PathGenParentTransform;

    protected GridPath PathFindingGrid = new GridPath();
    /// <summary>
    /// Access to path finding on the map
    /// </summary>
    public GridPath GetGridPath { get { return PathFindingGrid; } }

    void Awake()
    {
        if (Instance != null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "Multi GameManager");
            return;
        }
        Instance = this;

        GenerateMap();
    }

    public virtual void GenerateMap()
    {
        //Generate the map
        mapGenerator = GetComponent<MapGenerator>();
        mapGenerator.MapGen();

        //Generate the rooms
        roomGenerator = GetComponent<RoomGenerator>();
        roomGenerator.RoomGen(mapGenerator, PathGenParentTransform);

        // set the map for the any path finding needed
        PathFindingGrid.SetRoomGenerator(mapGenerator);
    }

}
