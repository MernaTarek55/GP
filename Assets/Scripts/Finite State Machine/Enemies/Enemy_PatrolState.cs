using UnityEngine;
using UnityEngine.AI;

public class Enemy_PatrolState : EntityState
{
    private NavMeshAgent enemyAgent;  // to let the enemy move
    private float walkRadius = 10f; // How far the enemy can walk
    private GameObject playerGO;


    public Enemy_PatrolState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;
        TryGetComponents(enemyGO);
    }

    public override void Enter()
    {
        base.Enter();
        if (enemyData.enemyType == EnemyData.EnemyType.LavaRobot) enemyAgent.isStopped = false;

    }

    public override void Update()
    {
        base.Update();
        if (enemyData.enemyType != EnemyData.EnemyType.Turret)
            SetRandomDestination();
    }

    public override void Exit()
    {
        base.Exit();
        if (enemyData.enemyType == EnemyData.EnemyType.LavaRobot)
        {
            enemyAgent.isStopped = true;
        }
    }

    //protected override void UpdateTurret()
    //{
    //    Debug.Log("Turret Patrol - No movement (stationary enemy)");
    //    // Turrets typically don't patrol
    //    SetRandomDestination();

        //}

        //protected override void UpdateBallDroid()
        //{
        //    Debug.Log("BallDroid Patrol");
        //    SetRandomDestination();
        //}

        //protected override void UpdateHumanoid()
        //{
        //    Debug.Log("Humanoid Patrol");
        //    SetRandomDestination();

        //    // Implement humanoid patrol logic (e.g., navmesh waypoints)
        //}

        //protected override void UpdateLavaRobot()
        //{
        //    Debug.Log("LavaRobot Patrol");

        //        SetRandomDestination();

        //    // Implement lava robot patrol logic
        //}


    private void TryGetComponents(GameObject enemyGO)
    {
        if (enemyGO.TryGetComponent(out NavMeshAgent eNav))
            enemyAgent = eNav;
        else
            Debug.LogWarning("Nav mesh not found");
    }

    void SetRandomDestination()
    {
        if (enemyAgent.remainingDistance < 1f && !enemyAgent.pathPending)
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += enemyGO.transform.position;

            NavMeshHit hit;
            // Try 30 times to find a valid position (avoids infinite loops)
            for (int i = 0; i < 30; i++)
            {
                if (NavMesh.SamplePosition(randomDirection, out hit, walkRadius, NavMesh.AllAreas))
                {
                    enemyAgent.SetDestination(hit.position);
                    return;
                }
                // If failed, try another random direction
                randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection += enemyGO.transform.position;
            }

            // If all attempts fail, just use current position
            enemyAgent.SetDestination(enemyGO.transform.position);
        }
    }

    public override void CheckStateTransitions(float distanceToPlayer)
    {
        if (distanceToPlayer <= enemyData.DetectionRange)
        {

            if (enemyData.enemyGroup == EnemyData.EnemyGroup.Chaser)
            {
                stateMachine.ChangeState(new Enemy_ChaseState(stateMachine, "Chase", enemyData, enemyGO, playerGO,enemy));
            }
            else
            {
                Debug.LogError("to attack");
                stateMachine.ChangeState(new Enemy_AttackState(stateMachine, "Attack", enemyData, enemyGO, playerGO));
            }
        }
    }
}
