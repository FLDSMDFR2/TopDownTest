using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOPoolManager : MonoBehaviour
{
    protected static Dictionary<Type, Queue<GameObject>> objPool = new Dictionary<Type, Queue<GameObject>>();
    protected static Transform myTransform;

    private void Start()
    {
        myTransform = this.transform;
    }

    public static GameObject GetObject(Type key, GameObject obj, Vector3 pos, Quaternion rot)
    {
        if (objPool.ContainsKey(key) && objPool[key].Count > 0)
        {
            var retObj = objPool[key].Dequeue();
            retObj.transform.position = pos;
            retObj.transform.rotation = rot;
            retObj.SetActive(true);
            return retObj;
        }

        return CreateObject(key, obj, pos, rot);
    }

    protected static GameObject CreateObject(Type key, GameObject obj, Vector3 pos, Quaternion rot)
    {
        GameObject retVal = null;
        foreach (Transform child in myTransform)
        {
            if (child.name == key.Name)
            {
                retVal = Instantiate(obj, pos, rot, child);
                break;
            }
        }

        if (retVal == null)
        {
            var parent = Instantiate(new GameObject(), myTransform);
            parent.name = key.Name;

            retVal = Instantiate(obj, pos, rot, parent.transform);
        }

        return retVal;
    }

    public static void AddObject(Type key, GameObject obj)
    {
        if (objPool.ContainsKey(key))
        {
            objPool[key].Enqueue(obj);
        }
        else
        {
            objPool.Add(key, new Queue<GameObject>());
            objPool[key].Enqueue(obj);
        }

        obj.SetActive(false);
    }
}
