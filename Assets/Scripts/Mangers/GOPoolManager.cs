using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOPoolManager : MonoBehaviour
{
    protected static Dictionary<string, Queue<GameObject>> objPool = new Dictionary<string, Queue<GameObject>>();
    protected static Transform myTransform;

    private void Start()
    {
        myTransform = this.transform;
    }

    public static GameObject GetObject(string key, GameObject obj, Vector3 pos, Quaternion rot)
    {
        if (objPool.ContainsKey(key) && objPool[key].Count > 0)
        {
            var retObj = objPool[key].Dequeue();
            var found = false;
            while (true)
            {
                //if not active
                if (!retObj.activeSelf)
                {
                    found = true;
                    break;
                }                 
                else
                {
                    if (objPool[key].Count > 0)
                        retObj = objPool[key].Dequeue();
                    else
                        break; // no more in queue
                }
                    
            }

            if (found)
            {
                retObj.transform.position = pos;
                retObj.transform.rotation = rot;
                retObj.SetActive(true);
                return retObj;
            }
        }

        return CreateObject(key, obj, pos, rot);
    }

    protected static GameObject CreateObject(string key, GameObject obj, Vector3 pos, Quaternion rot)
    {
        GameObject retVal = null;
        foreach (Transform child in myTransform)
        {
            if (child.name.Equals(key))
            {
                retVal = Instantiate(obj, pos, rot, child);
                break;
            }
        }

        if (retVal == null)
        {
            var parent = new GameObject(key);
            parent.transform.parent = myTransform;

            retVal = Instantiate(obj, pos, rot, parent.transform);
        }

        return retVal;
    }

    public static void AddObject(string key, GameObject obj)
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
