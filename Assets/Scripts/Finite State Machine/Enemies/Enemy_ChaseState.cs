﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class Enemy_ChaseState : EntityState
{
    private GameObject playerGO;
    private InvisibilitySkill invisibilitySkill;
    private NavMeshAgent enemyAgent;  // to let the enemy move
    private Rigidbody enemyRigidbody; // to add force for the beyblade
    private Rigidbody playerRigidbody; // to add force for the beyblade
    //private  readonly GameObject enemyGO;


    public Enemy_ChaseState(StateMachine stateMachine, string stateName, EnemyData enemyData,
                         GameObject enemyGO, GameObject playerGO, Enemy enemy)
       : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;
        this.enemyGO = enemyGO;

        this.enemy = enemy;  // Store the Enemy reference
        //TODO fix the enemy null situation
        if(enemy== null) { enemy = enemyGO.gameObject.GetComponent<Enemy>(); }
        TryGetComponents(this.playerGO);
        TryGetComponents(this.enemyGO);
    }

    public override void Update()
    {
        base.Update();
        if (enemyData.enemyGroup == EnemyData.EnemyGroup.Chaser)
        {
            if (invisibilitySkill.isInvisible)
            {
                Debug.Log("Player is invisible");
                stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
                return;
            }
            ChasePlayer();

        }
    }

    //protected override void UpdateTurret()
    //{
    //    Debug.Log("Turret Chase - No movement (stationary enemy)");
    //    // Turrets typically don't chase, so this can be empty
    //}

    //protected override void UpdateBallDroid()
    //{
    //    Debug.Log("BallDroid Chase");
    //    if (playerGO.GetComponent<InvisibilitySkill>().isInvisible)
    //    {
    //        Debug.Log("Player is invisible");
    //        stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO));
    //        return;
    //    }

    //    ChasePlayer();
    //}

    //protected override void UpdateHumanoid()
    //{
    //    Debug.Log("Humanoid Chase");
    //    // Implement humanoid chase logic here (e.g., navmesh movement)
    //}

    private void TryGetComponents(GameObject entityGO)
    {
        if (entityGO.CompareTag("Player"))
        {

            if (entityGO.TryGetComponent(out Rigidbody playerRB))
                playerRigidbody = playerRB;
            else
                Debug.LogWarning("playerRigidbody not found");
            if (entityGO.TryGetComponent(out InvisibilitySkill invisibilitySkill)) this.invisibilitySkill = invisibilitySkill;
            else Debug.LogWarning("invisibilitySkill not found");
           
        }
        else
        {
            if (entityGO.TryGetComponent(out NavMeshAgent eNav))
                enemyAgent = eNav;
            else
                Debug.LogWarning("Nav mesh not found");
            if (entityGO.TryGetComponent(out Rigidbody enemyRB))
                enemyRigidbody = enemyRB;
            else
                Debug.LogWarning("enemyRigidbody not found entity name"+ entityGO.name);
        }
        
    }
    private void ChasePlayer()
    {
        if (playerGO == null || enemyGO == null) return;
        if(enemyAgent == null) return;
        enemyAgent.SetDestination(playerGO.transform.position);
        /*float dirX = playerGO.transform.position.x - enemyGO.transform.position.x;
        float dirZ = playerGO.transform.position.z - enemyGO.transform.position.z;
        Vector3 direction = new Vector3(dirX, 0, dirZ).normalized;
        //Vector3 direction = (playerGO.transform.position - enemyGO.transform.position).normalized;
        enemyGO.transform.position += direction * enemyData.movementSpeed * Time.deltaTime;*/
        //Debug.Log(Vector3.Distance(playerGO.transform.position, enemyGO.transform.position));
        //if (playerGO.gameObject.tag)
        //{
        //    stateMachine.ChangeState(new Enemy_AttackState(stateMachine, "Attack", enemyData, enemyGO, playerGO));
        //}

        //if(enemyGo.gameObject.tag) {
    }

    //public override void OnCollisionEnter(Collision collision)
    //{
    //    //if (collision.gameObject.CompareTag("Player"))
    //    //{
    //    //    Debug.Log("Enemy collided with Player � transitioning to Attack state");

    //    //    if (enemyData.enemyType == EnemyData.EnemyType.Beyblade)
    //    //    {
    //    //        //CollidingFriction(collision);

    //    //    }

    //    //    stateMachine.ChangeState(new Enemy_AttackState(stateMachine, "Attack", enemyData, enemyGO, playerGO));
    //    //}
    //}

    //private void CollidingFriction(Collision collision)
    //{
    //    float impactForce = 1;

    //    // Calculate the push-back force (tweak multiplier as needed)
    //    Debug.LogWarning("pushBackForce: " + impactForce);

    //    // Determine relative X position
    //    float directionX =playerGO.transform.position.x - enemyGO.transform.position.x;
    //    float pushDirection = directionX >= 0 ? 1f : -1f;

    //    // Create push direction only on the X-axis
    //    Vector3 pushVector = new Vector3(Mathf.Sign(pushDirection), 0f, 0f);

    //    // Apply push force to both enemy and player
    //    enemyRigidbody.AddForce(-pushVector * impactForce, ForceMode.Impulse); // Enemy gets opposite force
    //    playerRigidbody.AddForce(pushVector * impactForce, ForceMode.Impulse); // Player gets force based on enemy's position

    //    // Optional: Add a temporary movement freeze for beyblade effect
    //    if (enemy != null)
    //    {
    //        enemy.StartEnemyCoroutine(BeybladeWaitAttack());
    //    }
    //}


  
    public override void CheckStateTransitions(float distanceToPlayer)
    {
        if (!invisibilitySkill.isInvisible)
        {


            if (distanceToPlayer > enemyData.DetectionRange)
            {
                if (enemyData.enemyType is EnemyData.EnemyType.LavaRobot or EnemyData.EnemyType.LavaRobotTypeB)
                {
                    stateMachine.ChangeState(new Enemy_PatrolState(stateMachine, "Patrol", enemyData, enemyGO, playerGO));
                }
                else
                {
                    stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
                }
            }
            else if (distanceToPlayer < .7f)
            {
                Debug.LogWarning("Player too close");
                stateMachine.ChangeState(new Enemy_AttackState(stateMachine, "Attack", enemyData, enemyGO, playerGO));
            }
        }
    }
}