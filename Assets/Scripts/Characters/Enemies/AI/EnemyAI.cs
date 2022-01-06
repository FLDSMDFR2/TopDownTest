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
    public struct Data
    {

    }

    public EnemyStates CurrentState = EnemyStates.Wander;
    public IEnemyStates EnemyWander = new EnemyWanderState();
    public IEnemyStates EnemyChase = new EnemyChaseState();

    [Header("Enemy AI")]
    public float SearchDetectRadius;
    public GameObject SearchSphere;

    public bool HasGravity = false;

    public float PathCheckRate;

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

    public void Update()
    {
        if (Disabled || Enemy == null)
            return;

        switch (CurrentState)
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
