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
    public MapGenerator GetMapGenerator { get { return mapGenerator; } }
    /// <summary>
    /// Room Generator
    /// </summary>
    protected RoomGenerator roomGenerator;
    /// <summary>
    /// Transform to place built map GO into
    /// </summary>
    [SerializeField]
    protected Transform MapObjectParent;

    protected GridPath PathFindingGrid = new GridPath();
    /// <summary>
    /// Access to path finding on the map
    /// </summary>
    public GridPath GetGridPath { get { return PathFindingGrid; } }

    [SerializeField]
    protected PathFinding_V2 PathV2;
    public PathFinding_V2 GetPathV2 { get { return PathV2; } }

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
        roomGenerator.RoomGen(mapGenerator, MapObjectParent);

        //TODO: REMOVE THIS OLD PATHFINDING
        PathFindingGrid.SetRoomGenerator(mapGenerator);

        // set the map for version 2 path finding
        PathV2.SetMap(mapGenerator);
    }

}
