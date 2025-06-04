using UnityEngine;
using UnityEngine.AI;

public class Enemy_PatrolState : EntityState
{
    private NavMeshAgent enemyAgent;  // to let the enemy move
    private float walkRadius = 10f; // How far the enemy can walk
    private GameObject playerGO;
    //private Enemy enemy;
    private int counter = 0;
    private InvisibilitySkill invisibilitySkill;

    //private int counter = 0;



    public Enemy_PatrolState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;

        TryGetComponents(enemyGO);
        TryGetComponents(playerGO);
    }

    public override void Enter()
    {
        base.Enter();
        if (enemyData.enemyType == EnemyData.EnemyType.OneArmedRobot)
            enemyAgent.SetDestination(enemy.NavTargets[counter].position);
        if (enemyData.enemyType != EnemyData.EnemyType.Turret && enemyData.enemyGroup == EnemyData.EnemyGroup.Shooter)
            enemyAgent.isStopped = false;
    }

    public override void Update()
    {
        base.Update();
        if (enemyData.enemyType != EnemyData.EnemyType.Turret)
        {
            if (enemyData.enemyType == EnemyData.EnemyType.OneArmedRobot)
                SetTargetsDestination();
            else
                SetRandomDestination();
        }

    }

    public override void Exit()
    {
        base.Exit();
        if (enemyData.enemyType != EnemyData.EnemyType.Turret && enemyData.enemyGroup == EnemyData.EnemyGroup.Shooter)
        {
            enemyAgent.isStopped = true;
        }
    }



    private void TryGetComponents(GameObject entityGO)
    {
        if (entityGO.CompareTag("Player"))
        {
            if (entityGO.TryGetComponent(out InvisibilitySkill invisibilitySkill))
                this.invisibilitySkill = invisibilitySkill;
            else
                Debug.LogWarning("invisibilitySkill not found on player");
        }
        else // Enemy components
        {
            if (entityGO.TryGetComponent(out NavMeshAgent eNav))
                enemyAgent = eNav;
            else
                Debug.LogWarning("Nav mesh not found");

            if (entityGO.TryGetComponent(out Enemy enemy))
                this.enemy = enemy;
            else
                Debug.LogWarning("Enemy component not found");
        }

    }

    void SetRandomDestination()
    {
        if (enemyAgent.remainingDistance < 1f && !enemyAgent.pathPending)
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += enemyGO.transform.position;

            NavMeshHit hit;
            for (int i = 0; i < 30; i++)
            {
                if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, NavMesh.AllAreas))
                {
                    enemyAgent.SetDestination(hit.position);
                    return;
                }
                randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection += enemyGO.transform.position;
            }
            enemyAgent.SetDestination(enemyGO.transform.position);
        }
    }
    void SetTargetsDestination()
    {

        if (enemyAgent.remainingDistance < enemyAgent.stoppingDistance)
        {
            if (counter >= enemy.NavTargets.Length)
            {
                counter = 0;
            }
            else
            {
                enemyAgent.SetDestination(enemy.NavTargets[counter].position);
                counter++;
            }
        }
    }

    public override void CheckStateTransitions(float distanceToPlayer)
    {
        // FIX: Add null check for invisibilitySkill
        bool playerVisible = invisibilitySkill == null || !invisibilitySkill.isInvisible;

        if (playerVisible)
        {
            if (distanceToPlayer <= enemyData.DetectionRange)
            {
                Debug.Log($"Enemy {enemyGO.name} detected player at distance {distanceToPlayer}, transitioning from Patrol");

                if (enemyData.enemyGroup == EnemyData.EnemyGroup.Chaser)
                {
                    Debug.Log("Transitioning to Chase state");
                    stateMachine.ChangeState(new Enemy_ChaseState(stateMachine, "Chase", enemyData, enemyGO, playerGO, enemy));
                }
                else
                {
                    Debug.Log("Transitioning to Attack state");
                    stateMachine.ChangeState(new Enemy_AttackState(stateMachine, "Attack", enemyData, enemyGO, playerGO));
                }
            }
        }
        else
        {
            Debug.Log("Player is invisible, staying in patrol");
        }
    }
}


