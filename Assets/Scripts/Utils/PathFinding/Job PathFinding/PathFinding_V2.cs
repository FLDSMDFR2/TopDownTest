using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PathFinding_V2 : MonoBehaviour
{
    protected MapGenerator mapGenerator;
    [ReadOnly]
    protected NativeArray<PathNode> mapArray;
    [ReadOnly]
    protected NativeArray<int2> NeightborOffSetArray;

    protected void Awake()
    {
        BuildNeightborOffArray();
    }

    /// <summary>
    /// TODO: I think we want to get all objects that are trying to path find bool on object maybe add them to the jobs list
    /// then update maybe a path array within them and let them run that... not sure but need to group pathfinding objects to geather
    /// </summary>
    public virtual List<PathFindingJobDetails> PerformPathFinding(List<PathFindingJobDetails> pathJobs)
    {
        // if we dont have a map generator then we didnt build the hashmap so exit path finding
        if (mapGenerator == null)
            return pathJobs;

        // create array for all jobs we want to complete
        var index = 0;
        NativeArray<JobHandle> jobHandlesArray = new NativeArray<JobHandle>(pathJobs.Count, Allocator.TempJob);

        foreach (var job in pathJobs)
        {
            //create the job 
            FindPathJob findPathJob = new FindPathJob
            {
                StartPos = CreateJobDetailNode(job.StartPos),
                EndPos = CreateJobDetailNode(job.EndPos),
                Map = mapArray,
                NeightborOffSetArray = NeightborOffSetArray,
                Results = new NativeList<int2>(Allocator.TempJob)
        };

            //save the job
            job.Job = findPathJob;
            //add the handle for the job to the array
            JobHandle handle = findPathJob.Schedule();
            jobHandlesArray[index++] = handle;
        }

        // complete all jobs
        JobHandle.CompleteAll(jobHandlesArray);

        //save off results and clean up arrays
        GetResluts(pathJobs);

        jobHandlesArray.Dispose();

        return pathJobs;
    }

    protected virtual PathNode CreateJobDetailNode(float3 pos)
    {
        var node = new PathNode();
        node.MapSizeX = mapGenerator.MapSizeX;
        node.MapSizeY = mapGenerator.MapSizeY;
        node.MapChunkSizeX = mapGenerator.MapChunkSizeX;
        node.MapChunkSizeY = mapGenerator.MapChunkSizeY;
        node.LocationKey = Round(pos);
        node.CalculateIndex();

        return node;
    }

    /// <summary>
    /// Set Map Generator and build hashset for pathfinding
    /// </summary>
    /// <param name="mg"></param>
    public virtual void SetMap(MapGenerator mg)
    {
        mapGenerator = mg;
        UpdateMapArray();
    }

    protected virtual void GetResluts(List<PathFindingJobDetails> jobs)
    {
        foreach (var job in jobs)
        {
            // Save the results from the job to a list, the values in results need to be reversed
            job.Path = new List<int2>(job.Job.Results.ToArray());
            job.Path.RemoveAt(job.Path.Count - 1);//remove start pos
            job.Path.Reverse();// path is build backwards from end so revers it
            job.Job.Results.Dispose();
        }
    }

    protected virtual void BuildNeightborOffArray()
    {
        NeightborOffSetArray = new NativeArray<int2>(8, Allocator.Persistent);
        NeightborOffSetArray[0] = MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Left]; //left
        NeightborOffSetArray[1] = MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Right]; //right
        NeightborOffSetArray[2] = MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Up];//Up
        NeightborOffSetArray[3] = MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.Down]; //Down
        NeightborOffSetArray[4] = MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.DownLeft];//Left Down
        NeightborOffSetArray[5] = MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.UpLeft]; //Left Up
        NeightborOffSetArray[6] = MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.DownRight]; //Right Down
        NeightborOffSetArray[7] = MapTraversal.NeighborsDirectionsAll[(int)MapTraversal.MapTraversalDirectionsIndex.UpRight]; //Right Up
    }
    /// <summary>
    /// Update path hash set based on Map Generator supplied
    /// </summary>
    public virtual void UpdateMapArray()
    {
        var size = (mapGenerator.MapSizeX * mapGenerator.MapChunkSizeX) * (mapGenerator.MapSizeY * mapGenerator.MapChunkSizeY);
        mapArray = new NativeArray<PathNode>(size, Allocator.Persistent);

        // if no map generator was set there is nothing to update so just return
        if (mapGenerator == null)
            return;

        // loop over all map location keys
        foreach (var mapLocKey in mapGenerator.GetRooms().Keys)
        {
            var chunk = mapGenerator.GetRooms();
            var loc1 = chunk[mapLocKey];
            //loop over all room locations in this map location
            foreach (var roomLocationKey in chunk[mapLocKey].RoomLocations.Keys)
            {
                var loc = mapGenerator.GetMap()[mapLocKey].RoomLocations[roomLocationKey];
                //create a path node for each room location
                PathNode node = new PathNode();
                node.MapSizeX = mapGenerator.MapSizeX;
                node.MapSizeY = mapGenerator.MapSizeY;
                node.MapChunkSizeX = mapGenerator.MapChunkSizeX;
                node.MapChunkSizeY = mapGenerator.MapChunkSizeY;
                node.LocationKey = roomLocationKey;
                node.CalculateIndex();
                node.IsPassable = loc.DetermineIsPassable();
                node.LocationMoveCost = loc.LocationTraversalCost;

                node.GCost = int.MaxValue;
                node.LastNodeIndex = - 1;

                mapArray[node.Index] = node;
            }
        }
    }

    public int2 Round(float3 loc)
    {

        int2 retval = new int2();

        if (loc.x % 1 >= .5)
        {
            retval.x = ((int)loc.x) + 1;
        }
        else
        {
            retval.x = ((int)loc.x);
        }

        if (loc.z % 1 >= .5)
        {
            retval.y = ((int)loc.z) + 1;
        }
        else
        {
            retval.y = ((int)loc.z);
        }

        return retval;
    }

    protected virtual void OnDestroy()
    {
        mapArray.Dispose();
        NeightborOffSetArray.Dispose();
    }
}
