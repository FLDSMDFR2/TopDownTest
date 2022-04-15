using UnityEngine;


public enum EnemyStates
{
    None,
    Wander,
    Chase,
    Attack
}

[RequireComponent(typeof(BaseEnemy))]
[RequireComponent(typeof(EnemyInputController))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    protected bool DebugEnabled = false;

    public EnemyStates CurrentState = EnemyStates.Wander;
    protected EnemyWanderState EnemyWander = new EnemyWanderState();
    protected EnemyChaseState EnemyChase = new EnemyChaseState();
    protected EnemyAttackState EnemyAttack = new EnemyAttackState();

    [Header("Enemy AI")]
    public float SearchRadius;

    public bool Disabled = false;

    [Header("Enemy Wander State")]
    public float WanderPathCheckRate;

    [Header("Enemy Chase State")]
    public float ChasePathCheckRate;

    [Header("Enemy Attack State")]

    [HideInInspector]
    public EnemyInputController Controller;
    [HideInInspector]
    public BaseEnemy Enemy;
    [HideInInspector]
    public FieldOfView FOV;
    [HideInInspector]
    public PathFindingJobDetails pathDtl = new PathFindingJobDetails();

    protected virtual void OnDrawGizmos()
    {
        if (Application.isPlaying && pathDtl.Path != null && pathDtl.Path.Count > 0 && DebugEnabled)
        {
            foreach (var pathLoc in pathDtl.Path)
            {
                Gizmos.DrawWireCube(new Vector3(pathLoc.x, 0, pathLoc.y), new Vector3(1, .1f, 1));
            }
        }
    }

    protected virtual void Start()
    {
        Controller = GetComponent<EnemyInputController>();
        Enemy = GetComponent<BaseEnemy>();
        FOV = Enemy.FOV;
        PathFindingPool.AddObjectForPathFinding(this.gameObject, pathDtl);

        InitStates();
    }
    protected virtual void InitStates()
    {
        EnemyWander = new EnemyWanderState();
        EnemyChase = new EnemyChaseState();
        EnemyAttack = new EnemyAttackState();

        EnemyWander.PathCheckRate = WanderPathCheckRate;
        EnemyChase.PathCheckRate = ChasePathCheckRate;
    }

    protected virtual void Update()
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
                CurrentState = EnemyAttack.PerformState(this);
                break;
            default:
                break;
        }
    }
}
