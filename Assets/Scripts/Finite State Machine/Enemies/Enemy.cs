using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    public StateMachine enemyStateMachine {  get; private set; }
    public Enemy_IdleState enemyIdleState {get; private set; }
    private void Awake()
    {
        enemyStateMachine = new StateMachine();
        enemyIdleState = new Enemy_IdleState(enemyStateMachine, "Enemy Idle", enemyData);
    }

    private void Start()
    {
        enemyStateMachine.Initalize(enemyIdleState);// And Start with it.
        enemyIdleState.getEnemyGO(gameObject);
    }

    private void Update()
    {
        enemyStateMachine.currentState.Update();
    }
}
