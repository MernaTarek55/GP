using UnityEngine;

public class Enemy_ChaseState : EntityState
{
    EnemyData _enemyData;
    GameObject playerGO;
    public Enemy_ChaseState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        _enemyData = enemyData;
        this.playerGO = playerGO;


    }

    public override void Update()
    {
        base.Update();
        Debug.Log("I am in enemy");
        if (_enemyData != null)
        {
            switch (_enemyData.enemyType)
            {
                case 0:
                    Debug.Log("Turret");
                    break;

                case (EnemyData.EnemyType)1:
                    Debug.Log("ballDroid");
                    ChasePlayer();
                    break;

                case (EnemyData.EnemyType)2:
                    Debug.Log("Humanoid");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Ana Null");
        }
    }
    void ChasePlayer()
    {
        if (playerGO == null || enemyGO == null) return;

        Vector3 direction = (playerGO.transform.position - enemyGO.transform.position).normalized;

       
        enemyGO.transform.position += direction * _enemyData.movementSpeed  * Time.deltaTime;


    }
}
