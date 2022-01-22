using Unity.Mathematics;

public struct PathNode
{
    public int MapSizeX;
    public int MapSizeY;
    public int MapChunkSizeX;
    public int MapChunkSizeY;
    /// <summary>
    /// Location key for this path node within the map
    /// </summary>
    public int2 LocationKey;

    public int Index;
    /// <summary>
    /// Move cost from start
    /// </summary>
    public int GCost;
    /// <summary>
    /// Estimated cost from start to end
    /// </summary>
    public int HCost;
    /// <summary>
    /// G + H cost
    /// </summary>
    public int FCost;
    /// <summary>
    /// If location is passable
    /// </summary>
    public bool IsPassable;
    /// <summary>
    /// The last nodes key we just came from
    /// </summary>
    public int LastNodeIndex;
    /// <summary>
    /// Added cost to move through locaton
    /// If path is impeded but not block we want to add extra cost here
    /// </summary>
    public int LocationMoveCost;

    public void CalculateFCost()
    {
        FCost = GCost + HCost;
    }

    public void CalculateIndex()
    {
        Index = LocationKey.x + LocationKey.y * (MapSizeX * MapChunkSizeX);
    }
}

