using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private GameObject playerGO;
    public Transform[] NavTargets;
    private NavMeshAgent agent;
    [Header("Enemy Components")]
    [SerializeField] private GameObject firePos;
    //TODO : make it get private set
    public ParticleSystem particleEffect; // particle system for enemy ball explosion

    #region Enemy Drops
    [Header("Drops")]
    public GameObject drop; // enemy drops for player to pick up the currency
    [Range(0, 1)] public float dropChance = 0.7f;
    public int minDrops = 1;
    public int maxDrops = 3;
    public Vector3[] dropForce;

    #endregion
    public StateMachine enemyStateMachine {  get; private set; }
    public Enemy_IdleState enemyIdleState {get; private set; }
    public Enemy_AttackState enemyAttackState {get; private set; }
    public Enemy_ChaseState enemyChaseState {get; private set; }
    public Enemy_PatrolState enemyPatrolState {get; private set; }
    public Death_State enemyDeath { get; private set; }
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyStateMachine = new StateMachine();
        enemyIdleState = new Enemy_IdleState(enemyStateMachine, "Enemy Idle", enemyData, gameObject, playerGO);
        enemyAttackState = new Enemy_AttackState(enemyStateMachine, "Enemy Attack", enemyData, gameObject, playerGO);
        enemyChaseState = new Enemy_ChaseState(enemyStateMachine, "Enemy Chase", enemyData, gameObject, playerGO, this);
        enemyPatrolState = new Enemy_PatrolState(enemyStateMachine, "Enemy Patrol", enemyData, gameObject, playerGO);
        enemyDeath = new Death_State(enemyStateMachine, "Enemy Death", enemyData, this);


    }

    private void Start()
    {

        if (enemyData.enemyGroup is EnemyData.EnemyGroup.Shooter && enemyData.enemyType is not EnemyData.EnemyType.Turret) { enemyStateMachine.Initalize(enemyPatrolState); }
        else enemyStateMachine.Initalize(enemyIdleState);
        
            //enemyAttackState.getfirePos(firePos);
            Debug.Log("START " + firePos);

    }

    private void Update()
    {
        
        //if (playerGO.GetComponent<InvisibilitySkill>().isInvisible)
        //{
        //Debug.Log("Player is invisible");
        //return;
        //}

        float distance = Vector3.Distance(gameObject.transform.position, playerGO.transform.position);
        enemyStateMachine.currentState.CheckStateTransitions(distance);
        enemyStateMachine.currentState.Update();
        /*if (distance <= enemyData.DetectionRange)
        {
            if (enemyData.enemyType == EnemyData.EnemyType.ballDroid)
            {
                enemyStateMachine.ChangeState(enemyChaseState);

                if (distance <= 2f)
                {
                    enemyStateMachine.ChangeState(enemyAttackState);
                }

            }
            else if (enemyData.enemyType == EnemyData.EnemyType.LavaRobot)
            {


                enemyStateMachine.ChangeState(enemyPatrolState);
            }
            else
            {
                enemyStateMachine.ChangeState(enemyAttackState);
            }
        }
        else if (enemyData.enemyType == EnemyData.EnemyType.LavaRobot || enemyData.enemyType == EnemyData.EnemyType.LavaRobotTypeB)
        {

            enemyStateMachine.ChangeState(enemyPatrolState);
        }
        else
        {
            enemyStateMachine.ChangeState(enemyIdleState);
        }*/
        //  }

    }
    public void Die()
    {

       
        enemyStateMachine.ChangeState(enemyDeath);
    }


    private void OnCollisionEnter(Collision collision)
    {
        // if (PoolManager.Instance == null)
        // {
        //     Debug.LogWarning("DropItemPool instance not found!");
        //     _ = Instantiate(itemPrefab, transform.position + Vector3.up, Quaternion.identity);
        //     return;
        // }

        // _ = transform.position + Vector3.up;
        // GameObject drop = PoolManager.Instance.SpawnFromPool(PoolType.DropItem,
        // transform.position + Vector3.up,
        //     Quaternion.identity
        // );

        // if (drop.TryGetComponent<Rigidbody>(out Rigidbody rb))
        // {
        //     rb.AddForce(new Vector3(
        //         Random.Range(dropForce[0].x, dropForce[1].x),
        //         Random.Range(dropForce[0].y, dropForce[1].y),
        //         Random.Range(dropForce[0].z, dropForce[1].z)
        //     ), ForceMode.Impulse);
        // }
        if (enemyStateMachine.currentState is EntityState state)
        {
            state.OnCollisionEnter(collision);
        }
    }

    public void StartEnemyCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public GameObject getfirePos()
    {
        return firePos;
    }

   
    public void getDropProperties(GameObject drop, float dropChance, int minDrops, int maxDrops, Vector3[] dropForce)
    {
        drop = this.drop;
        dropChance = this.dropChance;
        minDrops = this.minDrops;
        maxDrops = this.maxDrops;
        dropForce = this.dropForce;

        Debug.LogError(drop+" "+dropChance+" "+minDrops+" "+maxDrops+" "+dropForce);
    }



}
