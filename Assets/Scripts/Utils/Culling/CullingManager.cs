using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullingManager : MonoBehaviour
{
    [SerializeField]
    protected float boundsX;
    
    [SerializeField]
    protected float boundsZ;

    [SerializeField]
    protected float cullTime = 1f;

    [SerializeField]
    protected GameObject CullObject;

    void Start()
    {
        StartCoroutine(CullObjects());
    }

    protected IEnumerator CullObjects()
    {
        while(true)
        {
            var center = GetCenterPoint();
            foreach (Transform child in CullObject.transform)
            {
                if (child.position.x > center.x + boundsX || child.position.x < center.x - boundsX)
                {
                    child.gameObject.SetActive(false);
                }
                else if (child.position.z > center.z + boundsZ || child.position.z < center.z - boundsZ)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    child.gameObject.SetActive(true);
                }
            }

            yield return new WaitForSeconds(cullTime);
        }
    }


    protected Vector3 GetCenterPoint()
    {
        Vector3 retVal = Vector3.zero;
        if (PlayerManager.GetPlayers() == null || PlayerManager.GetPlayers().Count <= 0)
            return retVal;


        if (PlayerManager.GetPlayers().Count == 1)
        {
            retVal = PlayerManager.GetPlayers()[0].transform.position;
        }

        var bounds = new Bounds(PlayerManager.GetPlayers()[0].transform.position, Vector3.zero);
        foreach (var trans in PlayerManager.GetPlayers())
        {
            bounds.Encapsulate(trans.transform.position);
        }

        retVal = bounds.center;

        return retVal;
    }

}
