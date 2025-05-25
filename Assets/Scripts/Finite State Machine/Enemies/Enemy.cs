using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] GameObject playerGO;

    [Header("Enemy Components")]
    [SerializeField] GameObject firePos;
    [SerializeField] ParticleSystem enemyPS; // particle system for enemy ball explosion


    #region Enemy Drops
    [Header("Drops")]
    [SerializeField]  GameObject drop; // enemy drops for player to pick up the currency
    [SerializeField][Range(0, 1)] private float dropChance = 0.7f;
    [SerializeField]  int minDrops = 1;
    [SerializeField]  int maxDrops = 3;
    [SerializeField] Vector3[] dropForce;

    #endregion
    public StateMachine enemyStateMachine {  get; private set; }
    public Enemy_IdleState enemyIdleState {get; private set; }
    public Enemy_AttackState enemyAttackState {get; private set; }
    public Enemy_ChaseState enemyChaseState {get; private set; }
    public Enemy_PatrolState enemyPatrolState {get; private set; }
    private void Awake()
    {
        enemyStateMachine = new StateMachine();
        enemyIdleState = new Enemy_IdleState(enemyStateMachine, "Enemy Idle", enemyData, gameObject);
        enemyAttackState = new Enemy_AttackState(enemyStateMachine, "Enemy Attack", enemyData, gameObject, playerGO);
        enemyChaseState = new Enemy_ChaseState(enemyStateMachine, "Enemy Chase", enemyData, gameObject, playerGO);    
        enemyPatrolState = new Enemy_PatrolState(enemyStateMachine, "Enemy Patrol", enemyData, gameObject);
        

    }

    private void Start()
    {
        
        enemyStateMachine.Initalize(enemyIdleState);
        enemyAttackState.getfirePos(firePos);
        enemyAttackState.getParticleSystem(enemyPS);
        Debug.Log("START " + firePos);

    }

    private void Update()
    {
        enemyStateMachine.currentState.Update();
        if (playerGO.GetComponent<InvisibilitySkill>().isInvisible)
        {
            Debug.Log("Player is invisible");
            return;
        }
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
                    enemyStateMachine.ChangeState(enemyAttackState);
                }
            }
            else
            {
                enemyStateMachine.ChangeState(enemyIdleState);
            }
      //  }
        
    }
    public void OnDestroy()
    {
        SpawnDrops();
    }
    #region Enemy Drops
    private void SpawnSingleDrop(GameObject itemPrefab)
    {
        if (DropItemPool.Instance == null)
        {
            Debug.LogWarning("DropItemPool instance not found!");
            Instantiate(itemPrefab, transform.position + Vector3.up, Quaternion.identity);
            return;
        }
        Vector3 spawnPos = transform.position + Vector3.up;
        GameObject drop = DropItemPool.Instance.GetDropFromPool(
            transform.position + Vector3.up,
            Quaternion.identity
        );

        if (drop.TryGetComponent<Rigidbody>(out var rb))
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
        if (drop == null) return;
        if (Random.value > dropChance) return;

        int amount = Random.Range(minDrops, maxDrops + 1);
        for (int i = 0; i < amount; i++)
        {
            SpawnSingleDrop(drop);
        }
    }
    #endregion



}
