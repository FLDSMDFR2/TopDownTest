using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchZone : MonoBehaviour
{

    private List<Transform> _targets = new List<Transform>();

    protected SphereCollider seachCollider;

    void Awake()
    {
        seachCollider = this.GetComponent<SphereCollider>();
    }

    public void  SetSearchRadius(float radius)
    {
        seachCollider.radius = radius;
    }

    public Transform CheckTarget()
    {
        if (_targets.Count == 0)
            return null;
        
        while(_targets[0] == null)
        {
            _targets.RemoveAt(0);
        }

        return _targets[0];
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<BasePlayer>();
        // check if we hit a player 
        if (player != null )
        {
            _targets.Add(player.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.gameObject.GetComponent<BasePlayer>();
        // check if we hit a player 
        if (player!= null && _targets.Contains(player.transform))
        {
            _targets.Remove(player.transform);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, seachCollider.radius);
    }
}
