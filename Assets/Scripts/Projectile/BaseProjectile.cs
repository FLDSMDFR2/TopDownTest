using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{

    public float Speed = .5f;
    public float Damage = 5f;
    public CollisionDetection ImpactCollider;

    private int _characterId;
    private Vector3 Direction = Vector3.forward;

    private void Start()
    {
        ImpactCollider.OnCollisionEvent += OnImpactCollider;
    }

    void Update()
    {
        UpdateProjectile();
    }

    public virtual void SetCharacterId(int CharacterId)
    {
        _characterId = CharacterId;
    }

    public virtual void SetDirection(Vector3 dir)
    {
        Direction = dir;
    }

    protected virtual void UpdateProjectile()
    {

        float distanceToTravel = Speed * Time.deltaTime;

        transform.Translate(Direction * distanceToTravel, Space.World);
    }

    protected virtual void PlayerHit(BaseCharacter player)
    {

        if (player != null)
        {
            player.TakeDamage(Damage);
        }

        DestoryProjectial();
    }

    protected virtual void GroundHit()
    {
        DestoryProjectial();
    }

    protected virtual void DestoryProjectial()
    {
        Destroy(gameObject);
    }

    private void OnImpactCollider(Collider other)
    {

        if (other.gameObject.tag == "Triggers" || other.gameObject.tag == "Projectile")
        {
            return;
        }

        var player = other.gameObject.GetComponent<BaseCharacter>();
        // check if we hit a player 
        if (player != null)
        {
            //if we did make sure its not ourself
            if (player.CharacterID != _characterId)
            {
                PlayerHit(player);
            }
        }
        else
        {
            GroundHit();
        }
    }

    private void OnDestroy()
    {
        ImpactCollider.OnCollisionEvent -= OnImpactCollider;
    }
}
