using UnityEngine;

public class Enemy_PatrolState : EntityState
{
    public Enemy_PatrolState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
    }

    protected override void UpdateTurret()
    {
        Debug.Log("Turret Patrol - No movement (stationary enemy)");
        // Turrets typically don't patrol
    }

    protected override void UpdateBallDroid()
    {
        Debug.Log("BallDroid Patrol");
        // Implement ball droid patrol logic (e.g., waypoint movement)
    }

    protected override void UpdateHumanoid()
    {
        Debug.Log("Humanoid Patrol");
        // Implement humanoid patrol logic (e.g., navmesh waypoints)
    }
}