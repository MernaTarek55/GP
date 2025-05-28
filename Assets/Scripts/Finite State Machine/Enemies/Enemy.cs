using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private GameObject playerGO;
    private NavMeshAgent agent;
    [Header("Enemy Components")]
    [SerializeField] private GameObject firePos;
    [SerializeField] private ParticleSystem enemyPS; // particle system for enemy ball explosion


    #region Enemy Drops
    [Header("Drops")]
    [SerializeField] private GameObject drop; // enemy drops for player to pick up the currency
    [SerializeField][Range(0, 1)] private float dropChance = 0.7f;
    [SerializeField] private int minDrops = 1;
    [SerializeField] private int maxDrops = 3;
    [SerializeField] private Vector3[] dropForce;

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
        enemyIdleState = new Enemy_IdleState(enemyStateMachine, "Enemy Idle", enemyData, gameObject);
        enemyAttackState = new Enemy_AttackState(enemyStateMachine, "Enemy Attack", enemyData, gameObject, playerGO);
        enemyChaseState = new Enemy_ChaseState(enemyStateMachine, "Enemy Chase", enemyData, gameObject, playerGO);
        enemyPatrolState = new Enemy_PatrolState(enemyStateMachine, "Enemy Patrol", enemyData, gameObject);
        enemyDeath = new Death_State(enemyStateMachine, "Enemy Death", enemyData, gameObject);


    }

    private void Start()
    {

        enemyStateMachine.Initalize(enemyIdleState);
        if (enemyData.enemyType is EnemyData.EnemyType.LavaRobot or EnemyData.EnemyType.LavaRobotTypeB) { enemyStateMachine.ChangeState(enemyPatrolState); }
        enemyAttackState.getfirePos(firePos);
        enemyAttackState.getParticleSystem(enemyPS);
        Debug.Log("START " + firePos);

    }

    private void Update()
    {
        enemyStateMachine.currentState.Update();
        //if (playerGO.GetComponent<InvisibilitySkill>().isInvisible)
        //{
        //Debug.Log("Player is invisible");
        //return;
        //}
        float distance = Vector3.Distance(gameObject.transform.position, playerGO.transform.position);
        if (distance <= enemyData.DetectionRange)
        {
            if (enemyData.enemyType == EnemyData.EnemyType.ballDroid)
            {
                enemyStateMachine.ChangeState(enemyChaseState);

                    if (distance <= 1f) { enemyStateMachine.ChangeState(enemyAttackState); }
                }
                else
                {
                if (enemyData.enemyType == EnemyData.EnemyType.LavaRobot)
                    agent.isStopped = true;
                    enemyStateMachine.ChangeState(enemyAttackState);
                }
            }
            else if(enemyData.enemyType == EnemyData.EnemyType.LavaRobot)
            {

                agent.isStopped = false;

                enemyStateMachine.ChangeState(enemyPatrolState);
            }
            else
            {
                if (enemyData.enemyType is EnemyData.EnemyType.LavaRobot or EnemyData.EnemyType.LavaRobotTypeB)
                {
                    agent.isStopped = true;
                }

                enemyStateMachine.ChangeState(enemyAttackState);
            }
        }
        else if (enemyData.enemyType is EnemyData.EnemyType.LavaRobot or EnemyData.EnemyType.LavaRobotTypeB)
        {

            agent.isStopped = false;

            enemyStateMachine.ChangeState(enemyPatrolState);
        }
        else
        {
            enemyStateMachine.ChangeState(enemyIdleState);
        }
        //  }

    }
    public void Die()
    {

        SpawnDrops();
        enemyStateMachine.ChangeState(enemyDeath);
    }

    #region Enemy Drops 
    //TODO: Put the region in Enemy Death State
    private void SpawnSingleDrop(GameObject itemPrefab)
    {
        if (DropItemPool.Instance == null)
        {
            Debug.LogWarning("DropItemPool instance not found!");
            _ = Instantiate(itemPrefab, transform.position + Vector3.up, Quaternion.identity);
            return;
        }

        _ = transform.position + Vector3.up;
        GameObject drop = DropItemPool.Instance.GetDropFromPool(
            transform.position + Vector3.up,
            Quaternion.identity
        );

        if (drop.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(new Vector3(
                Random.Range(dropForce[0].x, dropForce[1].x),
                Random.Range(dropForce[0].y, dropForce[1].y),
                Random.Range(dropForce[0].z, dropForce[1].z)
            ), ForceMode.Impulse);
        }
    }

    public void SpawnDrops()
    {
        if (drop == null)
        {
            return;
        }

        if (Random.value > dropChance)
        {
            return;
        }

        int amount = Random.Range(minDrops, maxDrops + 1);
        for (int i = 0; i < amount; i++)
        {
            SpawnSingleDrop(drop);
        }
    }
    #endregion



}
