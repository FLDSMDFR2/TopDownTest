using UnityEngine;


public enum EnemyStates
{
    None,
    Wander,
    Chase,
    Attack
}
[RequireComponent(typeof(EnemyInputController))]
[RequireComponent(typeof(BaseEnemy))]
[RequireComponent(typeof(FieldOfView))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    protected bool DebugEnabled = false;

    public EnemyStates CurrentState = EnemyStates.Wander;
    public IEnemyStates EnemyWander = new EnemyWanderState();
    public IEnemyStates EnemyChase = new EnemyChaseState();

    [Header("Enemy AI")]
    public float SearchRadius;

    public float PathCheckRate;

    public bool Disabled = false;

    [HideInInspector]
    public EnemyInputController Controller;
    [HideInInspector]
    public BaseEnemy Enemy;
    [HideInInspector]
    public FieldOfView FOV;
    [HideInInspector]
    public PathFindingJobDetails pathDtl = new PathFindingJobDetails();

    void OnDrawGizmos()
    {
        if (Application.isPlaying && pathDtl.Path != null && pathDtl.Path.Count > 0 && DebugEnabled)
        {
            foreach (var pathLoc in pathDtl.Path)
            {
                Gizmos.DrawWireCube(new Vector3(pathLoc.x, 0, pathLoc.y), new Vector3(1, .1f, 1));
            }
        }
    }

    private void Start()
    {
        Controller = GetComponent<EnemyInputController>();
        Enemy = GetComponent<BaseEnemy>();
        FOV = GetComponent<FieldOfView>();
        PathFindingPool.AddObjectForPathFinding(this.gameObject, pathDtl);
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
