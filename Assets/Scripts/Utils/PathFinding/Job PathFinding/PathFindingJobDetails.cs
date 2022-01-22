using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PathFindingJobDetails 
{
    /// <summary>
    /// Object this path finding is for
    /// </summary>
    public object Key;
    /// <summary>
    /// Start position of path finding
    /// </summary>
    public float3 StartPos;
    /// <summary>
    /// End Position of path finding 
    /// </summary>
    public float3 EndPos;
    /// <summary>
    /// If we need to update the path
    /// </summary>
    public bool UpdatePath;
    /// <summary>
    /// referance to the job perfroming this path finding (if running as job)
    /// </summary>
    public FindPathJob Job;
    /// <summary>
    /// The found path
    /// </summary>
    public List<int2> Path;
}
