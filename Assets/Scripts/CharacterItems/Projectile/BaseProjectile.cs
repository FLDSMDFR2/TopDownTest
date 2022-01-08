using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseProjectile : MonoBehaviour
{
    public float Speed = .5f;
    public float Damage = 5f;

    protected Rigidbody body;
    protected int _characterId;
    protected Vector3 direction = Vector3.forward;
    protected float range = 1;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public virtual void Reset()
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }

    public virtual void SetCharacterId(int CharacterId)
    {
        _characterId = CharacterId;
    }

    public virtual void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    public virtual void SetRange(float r)
    {
        range = r;
        StartCoroutine(DestoryProjectialRange());
    }

    public virtual void Fire()
    {
        body.AddForce(direction * Speed, ForceMode.Impulse);
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
        StopCoroutine(DestoryProjectialRange());

        GOPoolManager.AddObject(this.GetType(), gameObject);
    }

    protected virtual IEnumerator DestoryProjectialRange()
    {
        yield return new WaitForSeconds(range);
        GOPoolManager.AddObject(this.GetType(), gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        HandleCollision(other);
    }

    protected virtual void HandleCollision(Collision other)
    {
        if (other.gameObject.tag == "Projectile")
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
}
