using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile(CompileSynchronously = true)]
public struct FindPathJob : IJob
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public PathNode StartPos;
    public PathNode EndPos;
    [ReadOnly]
    public NativeArray<PathNode> Map;
    [ReadOnly]
    public NativeArray<int2> NeightborOffSetArray;
    [WriteOnly]
    public NativeList<int2> Results;
    private PathNode helperNode;

    public void Execute()
    {
        //TODO: MAKE THIS ALLOCATION BETTER OR MORE DYNAMIC
        NativeHashMap<int,PathNode> jobMap = new NativeHashMap<int,PathNode>(2500, Allocator.Temp);

        //setup helper node for calculations
        helperNode = new PathNode();
        helperNode.MapSizeX = StartPos.MapSizeX;
        helperNode.MapSizeY = StartPos.MapSizeY;
        helperNode.MapChunkSizeX = StartPos.MapChunkSizeX;
        helperNode.MapChunkSizeY = StartPos.MapChunkSizeY;

        //setup starting node values
        PathNode startNode = Map[StartPos.Index];
        startNode.GCost = 0;
        startNode.HCost = CalculateDistanceCost(startNode.LocationKey, EndPos.LocationKey, 0);
        startNode.CalculateFCost();
        AddUpdateJobNode(startNode.Index, startNode, jobMap);

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        openList.Add(StartPos.Index);

        while (openList.Length > 0)
        {
            int currentIndex = GetLowestFCostNodeKey(openList, jobMap);
            PathNode currentNode = Map[currentIndex];

            if (currentIndex == EndPos.Index)
            {
                //found the end node
                break;
            }

            for (int i = 0; i < openList.Length; i++)
            {
                if (openList[i] == currentIndex)
                {
                    // remove from open list
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }

            // add to close list
            closedList.Add(currentIndex);

            for (int i = 0; i < NeightborOffSetArray.Length; i++)
            {
                int2 neightbourOffSet = NeightborOffSetArray[i];
                int2 neighbourPosition = new int2(currentNode.LocationKey.x + neightbourOffSet.x, currentNode.LocationKey.y + neightbourOffSet.y);

                if (!IsPositionInsideGrid(neighbourPosition))
                {
                    // pos is not in the grid move to the next one
                    continue;
                }

                helperNode.LocationKey = neighbourPosition;
                helperNode.CalculateIndex();
                int neighbourNodeIndex = helperNode.Index;

                if (closedList.Contains(neighbourNodeIndex))
                {
                    //already searched this node
                    continue;
                }

                PathNode neighbourNode = GetPathNode(neighbourNodeIndex, jobMap);
                if (!neighbourNode.IsPassable)
                {
                    //node is not passable
                    continue;
                }

                int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode.LocationKey, neighbourPosition, neighbourNode.LocationMoveCost);
                if (tentativeGCost < neighbourNode.GCost)
                {
                    neighbourNode.LastNodeIndex = currentNode.Index;
                    neighbourNode.GCost = tentativeGCost;
                    neighbourNode.CalculateFCost();
                    AddUpdateJobNode(neighbourNode.Index, neighbourNode, jobMap);

                    if (!openList.Contains(neighbourNode.Index))
                    {
                        neighbourNode.HCost = CalculateDistanceCost(neighbourNode.LocationKey, EndPos.LocationKey, 0);
                        neighbourNode.CalculateFCost();
                        AddUpdateJobNode(neighbourNode.Index, neighbourNode, jobMap);

                        openList.Add(neighbourNode.Index);
                    }
                }
            }
        }

        PathNode endNode = jobMap[EndPos.Index];
        if (endNode.LastNodeIndex == -1)
        {
            //no path found
            //Debug.Log("No Path found");
        }
        else
        {
            //found path
            CalculatePath(endNode, jobMap);
        }

        // Dispose
        openList.Dispose();
        closedList.Dispose();
        jobMap.Dispose();
    }

    private void CalculatePath(PathNode endNode, NativeHashMap<int, PathNode> jobMap)
    {
        if (endNode.LastNodeIndex != -1)
        { 
            Results.Add(endNode.LocationKey);
            PathNode currentNode = endNode;
            while (currentNode.Index != StartPos.Index)
            {
                PathNode cameFromNode = jobMap[currentNode.LastNodeIndex];
                Results.Add(cameFromNode.LocationKey);
                currentNode = cameFromNode;
            }
        }
    }

    private void AddUpdateJobNode(int key, PathNode node, NativeHashMap<int, PathNode> jobMap)
    {
        if (jobMap.ContainsKey(key))
        {
            jobMap[key] = node;
        }
        else
        {
            jobMap.Add(key,node);
        }
    }

    // Get path node from job map unless it has not been updated then grab the base Map node
    private PathNode GetPathNode(int index, NativeHashMap<int, PathNode> jobMap)
    {
        if (jobMap.ContainsKey(index))
        {
            return jobMap[index];
        }
        else
        {
            return Map[index];
        }
    }

    private int CalculateDistanceCost(int2 aPos, int2 bPos, int addedLocationMoveCost)
    {
        int xDistance = math.abs(aPos.x - bPos.x);
        int yDistance = math.abs(aPos.y - bPos.y);
        int remaining = math.abs(xDistance - yDistance);
        return (MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining) + addedLocationMoveCost;
    }

    private int GetLowestFCostNodeKey(NativeList<int> openList, NativeHashMap<int, PathNode> jobMap)
    {
        PathNode lowestCostPathNode = jobMap[openList[0]];
        for (int i = 1; i < openList.Length; i++)
        {
            PathNode testPathNode = jobMap[openList[i]];
            if (testPathNode.FCost < lowestCostPathNode.FCost)
            {
                lowestCostPathNode = testPathNode;
            }
        }

        return lowestCostPathNode.Index;
    }

    private bool IsPositionInsideGrid(int2 pos)
    {
        // if the map a key for this pos, its in the map
        return pos.x >= 0 &&
            pos.y >= 0 &&
            pos.x < (helperNode.MapSizeX * helperNode.MapChunkSizeX) &&
            pos.y < (helperNode.MapSizeY * helperNode.MapChunkSizeY);
    }
}