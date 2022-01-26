using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingPool : MonoBehaviour
{
    protected static Dictionary<object, PathFindingJobDetails> objectPathDicitonary = new Dictionary<object, PathFindingJobDetails>();
    protected PathFinding pathFinding = new PathFinding();

    protected void LateUpdate()
    {
        // THIS SHOULD HAPPEN AFTER ALL OBJECT COULD HAVE REQUEST
        // PATH OBJECTS SHOULD FLAG FOR PATH UPDATE IN UPDATE
        PerformPathUpdates();
        //PerformPathUpdates_v2();
    }

    protected virtual void PerformPathUpdates()
    {
        // build list for work
        var work = new List<PathFindingJobDetails>();
        foreach (var key in objectPathDicitonary.Keys)
        {
            if (objectPathDicitonary[key].UpdatePath)
                work.Add(objectPathDicitonary[key]);

            objectPathDicitonary[key].UpdatePath = false;
        }

        // perform the path finding
        work = pathFinding.PerformPathFinding(GameManager.Instance.GetGridPath, work);


        foreach (var w in work)
        {
            // this should just update the path objects in the dictionary
            AddObjectForPathFinding(w.Key, w);
        }
    }

    protected virtual void PerformPathUpdates_v2()
    {
        // build list for work
        var work = new List<PathFindingJobDetails>();
        foreach (var key in objectPathDicitonary.Keys)
        {
            if (objectPathDicitonary[key].UpdatePath)
                work.Add(objectPathDicitonary[key]);

            objectPathDicitonary[key].UpdatePath = false;
        }

        // perform the path finding
        work = GameManager.Instance.GetPathV2.PerformPathFinding(work);

        foreach (var w in work)
        {
            // this should just update the path objects in the dictionary
            AddObjectForPathFinding(w.Key, w);
        }
    }

    /// <summary>
    /// Register object with path finding pool
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="dtls"></param>
    public static void AddObjectForPathFinding(object obj, PathFindingJobDetails dtls)
    {
        if (objectPathDicitonary.ContainsKey(obj))
        {
            objectPathDicitonary[obj] = dtls;
        }
        else
        {
            dtls.Key = obj;
            objectPathDicitonary.Add(obj, dtls);
        }
    }

    public static PathFindingJobDetails GetObjectForPathFinding(object obj)
    {
        if (objectPathDicitonary.ContainsKey(obj))
        {
            return objectPathDicitonary[obj];
        }

        return null;
    }

    /// <summary>
    /// Flag object as needing path updated
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>true if object is in pool</returns>
    public static bool RequestPathUpdate(object obj)
    {
        if (objectPathDicitonary.ContainsKey(obj))
        {
            objectPathDicitonary[obj].UpdatePath = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Remove object from path finding
    /// </summary>
    /// <param name="obj"></param>
    public static void RemoveObjectForPathFinding(object obj)
    {
        if (objectPathDicitonary.ContainsKey(obj))
        {
            objectPathDicitonary.Remove(obj);
        }
    }
}
