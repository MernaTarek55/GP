using UnityEngine;

public class Enemy_ChaseState : EntityState
{
    private readonly GameObject playerGO;

    public Enemy_ChaseState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;
    }

    protected override void UpdateTurret()
    {
        Debug.Log("Turret Chase - No movement (stationary enemy)");
        // Turrets typically don't chase, so this can be empty
    }

    protected override void UpdateBallDroid()
    {
        Debug.Log("BallDroid Chase");
        if (playerGO.GetComponent<InvisibilitySkill>().isInvisible)
        {
            Debug.Log("Player is invisible");
            stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO));
            return;
        }

        ChasePlayer();
    }

    protected override void UpdateHumanoid()
    {
        Debug.Log("Humanoid Chase");
        // Implement humanoid chase logic here (e.g., navmesh movement)
    }

    private void ChasePlayer()
    {
        if (playerGO == null || enemyGO == null)
        {
            return;
        }

        Vector3 direction = (playerGO.transform.position - enemyGO.transform.position).normalized;
        enemyGO.transform.position += direction * enemyData.movementSpeed * Time.deltaTime;
    }
}