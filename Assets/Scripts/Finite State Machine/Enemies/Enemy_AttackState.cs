using UnityEngine;

public class Enemy_AttackState : EntityState
{
    EnemyData _enemyData;
    public Enemy_AttackState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        _enemyData = enemyData;



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
}
