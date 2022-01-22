using Unity.Mathematics;

public class MapTraversal
{
    public enum MapTraversalDirectionsIndex
    {
        Right= 0,
        Left,
        Up,
        Down,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    }

    /// <summary>
    /// All directions around a point
    /// </summary>
    public static readonly int2[] NeighborsDirectionsAll = new[] {
        new int2(1, 0), // Right
        new int2(-1, 0),//Left
        new int2(0, 1), // Up
        new int2(0, -1),// Down
        new int2(1, 1), // diagonal top right
        new int2(-1, 1), // diagonal top left
        new int2(1, -1), // diagonal bottom right
        new int2(-1, -1) // diagonal bottom left
     };

    /// <summary>
    /// Orthogonal direction from point
    /// </summary>
    public static readonly int2[] NeighborsDirectionsOrtho = new[] {
        new int2(1, 0), // Right
        new int2(-1, 0),//Left
        new int2(0, 1), // Up
        new int2(0, -1)// Down
     };

    /// <summary>
    /// Diagonal direction from point
    /// </summary>
    public static readonly int2[] NeighborsDirectionsDiag= new[] {
        new int2(1, 1), // diagonal top right
        new int2(-1, 1), // diagonal top left
        new int2(1, -1), // diagonal bottom right
        new int2(-1, -1) // diagonal bottom left
     };

}
