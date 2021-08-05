using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public delegate void OnCollision(Collider collider);
    public event OnCollision OnCollisionEvent;


    private void OnTriggerEnter(Collider other)
    {
        if (OnCollisionEvent != null)
        {
            OnCollisionEvent(other);
        }
    }
}
