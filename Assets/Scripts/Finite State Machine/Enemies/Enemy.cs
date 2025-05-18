using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] GameObject playerGO;
    public StateMachine enemyStateMachine {  get; private set; }
    public Enemy_IdleState enemyIdleState {get; private set; }
    public Enemy_AttackState enemyAttackState {get; private set; }
    public Enemy_ChaseState enemyChaseState {get; private set; }
    public Enemy_PatrolState enemyPatrolState {get; private set; }
    private void Awake()
    {
        enemyStateMachine = new StateMachine();
        enemyIdleState = new Enemy_IdleState(enemyStateMachine, "Enemy Idle", enemyData, gameObject);
        enemyAttackState = new Enemy_AttackState(enemyStateMachine, "Enemy Attack", enemyData, gameObject);
        enemyChaseState = new Enemy_ChaseState(enemyStateMachine, "Enemy Chase", enemyData, gameObject);    
        enemyPatrolState = new Enemy_PatrolState(enemyStateMachine, "Enemy Patrol", enemyData, gameObject);
    }

    private void Start()
    {
        enemyStateMachine.Initalize(enemyIdleState);// And Start with it.
    }

    private void Update()
    {
        enemyStateMachine.currentState.Update();
    }
   
}
