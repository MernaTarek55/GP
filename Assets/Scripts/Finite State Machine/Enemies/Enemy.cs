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
    private HealthComponent healthComponent;

    [Header("Enemy Components")]
    [SerializeField] private GameObject firePos;
    public ParticleSystem particleEffect;
    public Material dissolveMaterial;

    #region Enemy Drops
    [Header("Drops")]
    public GameObject drop;
    [Range(0, 1)] public float dropChance = 0.7f;
    public int minDrops = 1;
    public int maxDrops = 3;
    public Vector3[] dropForce;
    #endregion

    public StateMachine enemyStateMachine { get; private set; }
    public Enemy_IdleState enemyIdleState { get; private set; }
    public Enemy_AttackState enemyAttackState { get; private set; }
    public Enemy_ChaseState enemyChaseState { get; private set; }
    public Enemy_PatrolState enemyPatrolState { get; private set; }
    public Death_State enemyDeath { get; private set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        healthComponent = GetComponent<HealthComponent>();

        enemyStateMachine = new StateMachine();
        enemyIdleState = new Enemy_IdleState(enemyStateMachine, "Enemy Idle", enemyData, gameObject, playerGO);
        enemyAttackState = new Enemy_AttackState(enemyStateMachine, "Enemy Attack", enemyData, gameObject, playerGO);
        enemyChaseState = new Enemy_ChaseState(enemyStateMachine, "Enemy Chase", enemyData, gameObject, playerGO, this);
        enemyPatrolState = new Enemy_PatrolState(enemyStateMachine, "Enemy Patrol", enemyData, gameObject, playerGO);
        enemyDeath = new Death_State(enemyStateMachine, "Enemy Death", enemyData, this);

        // Subscribe to the health component's death event
        if (healthComponent != null)
        {
            healthComponent.OnDeath += HandleDeath;
        }
    }

    private void Start()
    {
        if (enemyData.enemyGroup == EnemyData.EnemyGroup.Shooter && enemyData.enemyType != EnemyData.EnemyType.Turret)
        {
            enemyStateMachine.Initalize(enemyPatrolState);
        }
        else
        {
            enemyStateMachine.Initalize(enemyIdleState);
        }

        Debug.Log("START " + firePos);
    }

    private void Update()
    {
        float distance = Vector3.Distance(gameObject.transform.position, playerGO.transform.position);

        enemyStateMachine.currentState.Update();
        enemyStateMachine.currentState.CheckStateTransitions(distance);
    }

    private void HandleDeath()
    {
        Debug.LogWarning("Enemy " + name + " is dying via delegate");
        enemyStateMachine.ChangeState(enemyDeath);
    }

    public void Die()
    {
        Debug.LogWarning("Spawning drops for " + name);
        enemyStateMachine.ChangeState(enemyDeath);
    }

    private void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= HandleDeath;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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

        Debug.LogError(drop + " " + dropChance + " " + minDrops + " " + maxDrops + " " + dropForce);
    }
    
    public void PlayDeathEffect()
    {
        if (particleEffect != null)
        {
            Instantiate(particleEffect, this.transform.position, Quaternion.identity);
        }
    }
    
    public void ChangeEnemyMaterial()
    {
        this.GetComponent<Renderer>().material = dissolveMaterial;
        EnemyDissolve();
    }

    public void EnemyDissolve()
    {
        // increase the dissolve of the material to make it fade
        //this.GetComponent<Renderer>().material;
    }
}