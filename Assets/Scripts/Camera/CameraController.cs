using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    protected List<Transform> targets;

    public float MaxMoveSpeed = 10f;
    public float SmoothTime = .5f;

    public float ZoomSpeed = 50f;
    public float ZoomLimiter = 50f;
    public float MaxZoom = 10f;
    public float MinZoom = 40f;
    
    public Vector3 OffSet;

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        AddTargets();
    }

    protected virtual void AddTargets()
    {
        targets = new List<Transform>();
        foreach (BasePlayer p in PlayerManager.GetPlayers())
        {
            if (!targets.Contains(p.transform))
            {
                targets.Add(p.transform);
            }
        }

        if (targets != null || targets.Count > 0)
        {
            transform.position = MoveToPos();
        }
    }

    void LateUpdate()
    {
        if (targets == null || targets.Count <= 0)
        {
            AddTargets();
            return;
        }

        Move();
        Zoom();

    }

    protected virtual void Move()
    {
        //transform.position = Vector3.Lerp(transform.position, centerPoint + OffSet, SmoothTime);
        transform.position = Vector3.SmoothDamp(transform.position, MoveToPos(), ref velocity, SmoothTime, MaxMoveSpeed); ;
    }

    protected virtual Vector3 MoveToPos()
    {
        Vector3 centerPoint = GetCenterPoint();
        centerPoint += GetMousePos() + OffSet;

        return centerPoint;
    }

    protected virtual Vector3 GetMousePos()
    {
        Vector2 retVal = cam.ScreenToViewportPoint(Input.mousePosition);
        retVal *= 2;
        retVal -= Vector2.one;
        var max = 0.9f;
        if (Mathf.Abs(retVal.x) > max || Mathf.Abs(retVal.y) > max)
        {
            retVal = retVal.normalized;
        }

        return new Vector3(retVal.x * 5,0f,retVal.y * 5);
    }

    protected virtual void Zoom()
    {
        float newZoom = Mathf.Lerp(MaxZoom, MinZoom, GetGreatestDistance() / ZoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }


    protected virtual Vector3 GetCenterPoint()
    {
        Vector3 retVal = Vector3.zero;
        if (targets == null || targets.Count <= 0)
            return retVal;


        if (targets.Count == 1)
        {
            retVal = targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (Transform trans in targets)
        {
            bounds.Encapsulate(trans.position);
        }

        retVal = bounds.center;

        return retVal;
    }

    protected virtual float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (Transform trans in targets)
        {
            bounds.Encapsulate(trans.position);
        }

        float retVal = bounds.size.x;
        if (bounds.size.y > bounds.size.x)
        {
            retVal = bounds.size.y  + (OffSet.y);
        }

        return retVal;
    }
}
