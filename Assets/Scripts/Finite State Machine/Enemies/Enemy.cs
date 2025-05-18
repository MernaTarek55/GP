using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    [SerializeField] GameObject playerGO;
    [SerializeField] GameObject firePos;
    public StateMachine enemyStateMachine {  get; private set; }
    public Enemy_IdleState enemyIdleState {get; private set; }
    public Enemy_AttackState enemyAttackState {get; private set; }
    public Enemy_ChaseState enemyChaseState {get; private set; }
    public Enemy_PatrolState enemyPatrolState {get; private set; }
    private void Awake()
    {
        Debug.Log("AWAKE " + firePos);
        enemyStateMachine = new StateMachine();
        enemyIdleState = new Enemy_IdleState(enemyStateMachine, "Enemy Idle", enemyData, gameObject);
        enemyAttackState = new Enemy_AttackState(enemyStateMachine, "Enemy Attack", enemyData, gameObject, playerGO);
        enemyChaseState = new Enemy_ChaseState(enemyStateMachine, "Enemy Chase", enemyData, gameObject, playerGO);    
        enemyPatrolState = new Enemy_PatrolState(enemyStateMachine, "Enemy Patrol", enemyData, gameObject);
        Debug.Log("AWAKE AFTER STATE CONSTRUCTOR" + firePos);

    }

    private void Start()
    {
        
        enemyStateMachine.Initalize(enemyIdleState);// And Start with it.
        enemyAttackState.getPlayer(playerGO);
        enemyAttackState.getfirePos(firePos);
        Debug.Log("START " + firePos);

    }

    private void Update()
    {
        enemyStateMachine.currentState.Update();
        float distance = Vector3.Distance(gameObject.transform.position, playerGO.transform.position);
        if (distance <= enemyData.DetectionRange)
        {
            if(enemyData.enemyType == EnemyData.EnemyType.ballDroid)
            {
                enemyStateMachine.ChangeState(enemyChaseState);
                if (distance <= 1f)
                {
                    enemyStateMachine.ChangeState(enemyAttackState);
                }
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
        }
}
