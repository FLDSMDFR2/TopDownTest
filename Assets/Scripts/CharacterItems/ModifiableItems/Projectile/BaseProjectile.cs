using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseProjectile : ModifiableItem, IPoolable
{
    [Header("Base Projectile")]
    /// <summary>
    /// Id of this object for object pooling
    /// </summary>
    [SerializeField]
    protected string PoolingID;
    /// <summary>
    /// Character id of who shot the projectile
    /// </summary>
    protected int characterId;
    /// <summary>
    /// Speed of Projectile
    /// </summary>
    protected float speed;
    /// <summary>
    /// Damage of projectile
    /// </summary>
    protected float damage;
    /// <summary>
    /// Range of projectile
    /// </summary>
    protected float range;
    /// <summary>
    /// Body to move with physics
    /// </summary>
    protected Rigidbody body;
    /// <summary>
    /// Direction of travel
    /// </summary>
    protected Vector3 direction = Vector3.forward;
    /// <summary>
    /// start pos
    /// </summary>
    protected Vector3 startPos;

    protected override void PerformAwake()
    {
        base.PerformAwake();
        body = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Init the Projectile before firing
    /// </summary>
    /// <param name="CharacterId"></param>
    /// <param name="dir"></param>
    /// <param name="range"></param>
    /// <param name="speed"></param>
    /// <param name="damage"></param>
    public virtual void InitProjectile(int CharacterId, Vector3 dir, float range, float speed, float damage)
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        characterId = CharacterId;
        direction = dir;

        this.speed = speed;
        this.damage = damage;
        this.range = range;

        startPos = transform.position;
    }

    /// <summary>
    /// Fire the Projectile
    /// </summary>
    public virtual void Fire()
    {
        body.AddForce(direction * speed, ForceMode.Impulse);
        StartCoroutine(DestoryProjectialRange());
    }

    #region Destruction
    protected virtual void DestoryProjectial()
    {
        StopCoroutine(DestoryProjectialRange());

        GOPoolManager.AddObject(GetPoolId(), gameObject);
    }

    protected virtual IEnumerator DestoryProjectialRange()
    {
        // while we are less then range just yeild
        while (Vector3.Distance(startPos, transform.position) < range)
        {
            yield return null;
        }
        GOPoolManager.AddObject(GetPoolId(), gameObject);
    }
    #endregion

    #region Collision
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
            if (player.ID != characterId)
            {
                PlayerHit(player);
            }
        }
        else
        {
            GroundHit();
        }
    }
    protected virtual void PlayerHit(BaseCharacter player)
    {

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        DestoryProjectial();
    }

    protected virtual void GroundHit()
    {
        DestoryProjectial();
    }
    #endregion

    public virtual string GetPoolId()
    {
        return PoolingID;
    }
}
