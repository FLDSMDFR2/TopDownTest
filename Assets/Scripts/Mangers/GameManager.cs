using UnityEngine;

public enum GameDifficulty
{
    Easy,
    Medium,
    Hard
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    protected GameDifficulty Difficulty;

    [SerializeField]
    protected MapGenerator roomGenerator;
    [SerializeField]
    protected Transform PathGenParentTransform;

    protected GridPath PathFindingGrid = new GridPath();
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
        roomGenerator.RoomGen(PathGenParentTransform, Difficulty);
        // set the map for the any path finding needed
        PathFindingGrid.SetRoomGenerator(roomGenerator);
    }

}
