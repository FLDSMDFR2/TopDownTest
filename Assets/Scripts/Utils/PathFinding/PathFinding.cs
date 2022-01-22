using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;

public class PriorityQueue<T>
{
    // From Red Blob: I'm using an unsorted array for this example, but ideally this
    // would be a binary heap. Find a binary heap class:
    // * https://bitbucket.org/BlueRaja/high-speed-priority-queue-for-c/wiki/Home
    // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
    // * http://xfleury.github.io/graphsearch.html
    // * http://stackoverflow.com/questions/102398/priority-queue-in-net

    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
    }

    // Returns the Location that has the lowest priority
    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

// Now that all of our classes are in place, we get get
// down to the business of actually finding a path.
public class PathFinding
{
    // Someone suggested making this a 2d field.
    // That will be worth looking at if you run into performance issues.
    public Dictionary<int2, int2> cameFrom = new Dictionary<int2, int2>();
    public Dictionary<int2, float> costSoFar = new Dictionary<int2, float>();

    private int2 start;
    private int2 goal;

    public virtual List<PathFindingJobDetails> PerformPathFinding(GridPath grid, List<PathFindingJobDetails> pathJobs)
    {
        foreach (var job in pathJobs)
        {
            FindPath(grid, job.StartPos, job.EndPos);
            job.Path = GetPath();
        }

        return pathJobs;
    }

    // Conduct the A* search
    public virtual void FindPath(GridPath grid, float3 s, float3 g)
    {
        start = grid.Round(s);
        goal = grid.Round(g);

        cameFrom.Clear();
        costSoFar.Clear();

        // frontier is a List of key-value pairs:
        // Location, (float) priority
        var frontier = new PriorityQueue<int2>();
        // Add the starting location to the frontier with a priority of 0
        frontier.Enqueue(start, 0f);

        cameFrom.Add(start, start); // is set to start, None in example
        costSoFar.Add(start, 0f);

        while (frontier.Count > 0f)
        {
            // Get the Location from the frontier that has the lowest
            // priority, then remove that Location from the frontier
            int2 current = frontier.Dequeue();

            // If we're at the goal Location, stop looking.
            if (current.Equals(goal)) break;

            // Neighbors will return a List of valid tile Locations
            // that are next to, diagonal to, above or below current
            foreach (var neighbor in grid.Neighbors(current))
            {

                // If neighbor is diagonal to current, graph.Cost(current,neighbor)
                // will return Sqrt(2). Otherwise it will return only the cost of
                // the neighbor, which depends on its type, as set in the TileType enum.
                // So if this is a normal floor tile (1) and it's neighbor is an
                // adjacent (not diagonal) floor tile (1), newCost will be 2,
                // or if the neighbor is diagonal, 1+Sqrt(2). And that will be the
                // value assigned to costSoFar[neighbor] below.
                float newCost = costSoFar[current] + grid.Cost(current, neighbor);

                // If there's no cost assigned to the neighbor yet, or if the new
                // cost is lower than the assigned one, add newCost for this neighbor
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {

                    // If we're replacing the previous cost, remove it
                    if (costSoFar.ContainsKey(neighbor))
                    {
                        costSoFar.Remove(neighbor);
                        cameFrom.Remove(neighbor);
                    }

                    costSoFar.Add(neighbor, newCost);
                    cameFrom.Add(neighbor, current);
                    float priority = newCost + Heuristic(neighbor, goal);
                    frontier.Enqueue(neighbor, priority);
                }
            }
        }

    }

    static public float Heuristic(int2 a, int2 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Return a List of Locations representing the found path
    public virtual List<int2> GetPath()
    {

        List<int2> path = new List<int2>();
        int2 current = goal;

        while (!current.Equals(start))
        {
            if (!cameFrom.ContainsKey(current))
            {
                TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "cameFrom does not contain current ["+ current.ToString() + "].");
                return new List<int2>();
            }
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }
}