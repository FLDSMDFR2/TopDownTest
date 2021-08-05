using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyStates
{
    None,
    Wander,
    Chase,
    Attack
}


public class EnemyAI : MonoBehaviour
{

    protected EnemyStates CurrentState = EnemyStates.Wander;
    protected IEnemyStates EnemyWander = new EnemyWanderState();
    protected IEnemyStates EnemyChase = new EnemyChaseState();

    public PathFinding pathFinding = new PathFinding();

    [Header("Enemy AI")]
    public float SearchDetectRadius;
    public GameObject SearchSphere;

    public bool HasGravity = false;

    public bool Disabled = false;

    [HideInInspector]
    public EnemyInputController Controller;
    [HideInInspector]
    public BaseEnemy Enemy;
    [HideInInspector]
    public EnemySearchZone SearchZone;

    private void Start()
    {
        Controller = GetComponent<EnemyInputController>();
        Controller.SetHasGravity(HasGravity);
        Enemy = GetComponent<BaseEnemy>();
        SearchZone = SearchSphere.GetComponent<EnemySearchZone>();
        SearchZone.SetSearchRadius(SearchDetectRadius);
    }

    private void Update()
    {
        if (Disabled  || Enemy == null)
            return;

        switch(CurrentState)
        {
            case EnemyStates.None:
                
                break;
            case EnemyStates.Wander:
                CurrentState = EnemyWander.PerformState(this);
                break;
            case EnemyStates.Chase:
                CurrentState = EnemyChase.PerformState(this);
                break;
            case EnemyStates.Attack:

                break;
            default:
                break;           
        }
    }
}
