using Unity.VisualScripting;
using UnityEngine;

public class Enemy_ChaseState : EntityState
{
    private GameObject playerGO;
    private InvisibilitySkill invisibilitySkill;

    public Enemy_ChaseState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;
        TryGetComponents(this.playerGO);

    }

    public override void Update()
    {
        base.Update();
        if (enemyData.enemyGroup == EnemyData.EnemyGroup.Chaser)
        {
            //if (invisibilitySkill.isInvisible)
            //{
                //Debug.Log("Player is invisible");
                //stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
                //return;
            //}
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

        if (entityGO.TryGetComponent(out InvisibilitySkill invisibilitySkill)) this.invisibilitySkill = invisibilitySkill;
        else Debug.LogWarning("invisibilitySkill not found");
    }
    private void ChasePlayer()
    {
        if (playerGO == null || enemyGO == null) return;

        float dirX = playerGO.transform.position.x - enemyGO.transform.position.x;
        float dirZ = playerGO.transform.position.z - enemyGO.transform.position.z;
        Vector3 direction = new Vector3(dirX, 0, dirZ).normalized;
        //Vector3 direction = (playerGO.transform.position - enemyGO.transform.position).normalized;
        enemyGO.transform.position += direction * enemyData.movementSpeed * Time.deltaTime;
        //Debug.Log(Vector3.Distance(playerGO.transform.position, enemyGO.transform.position));
        //if (playerGO.gameObject.tag)
        //{
        //    stateMachine.ChangeState(new Enemy_AttackState(stateMachine, "Attack", enemyData, enemyGO, playerGO));
        //}

        //if(enemyGo.gameObject.tag) {
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy collided with Player � transitioning to Attack state");
            stateMachine.ChangeState(new Enemy_AttackState(stateMachine, "Attack", enemyData, enemyGO, playerGO));
        }
    }

    public override void CheckStateTransitions(float distanceToPlayer)
    {
        //if (invisibilitySkill.isInvisible)
        //{
        //    stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
        //    return;
        //}

        if (distanceToPlayer <= 2f)
        {
            stateMachine.ChangeState(new Enemy_AttackState(stateMachine, "Attack", enemyData, enemyGO, playerGO));
        }
        else if (distanceToPlayer > enemyData.DetectionRange)
        {
            if (enemyData.enemyType == EnemyData.EnemyType.LavaRobot ||
                enemyData.enemyType == EnemyData.EnemyType.LavaRobotTypeB)
            {
                stateMachine.ChangeState(new Enemy_PatrolState(stateMachine, "Patrol", enemyData, enemyGO, playerGO));
            }
            else
            {
                stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
            }
        }
    }
}