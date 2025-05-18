using UnityEngine;
using DG.Tweening;

public class Enemy_IdleState : EntityState
{
    EnemyData _enemyData;
    bool _isRotating = false;
    GameObject enemyGO;

    public Enemy_IdleState(StateMachine stateMachine, string stateName, EnemyData enemyData)
        : base(stateMachine, stateName, enemyData)
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
                case 0: // Turret
                    Debug.Log("Turret");
                    if (!_isRotating)
                    {
                        RotateWithTween();
                    }
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
            Debug.Log("Ana Null");
        }
    }

    private void RotateWithTween()
    {
        _isRotating = true;



        Transform turret = enemyGO.transform;

        // Rotate to Y = 180
        turret.DORotate(new Vector3(-90, 0, 160), 4f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // Wait 1 second, then rotate back
                DOVirtual.DelayedCall(0f, () =>
                {
                    turret.DORotate(new Vector3(-90, 0, 0), 4f)
                          .SetEase(Ease.Linear)
                          .OnComplete(() =>
                          {
                              _isRotating = false;
                          });
                });
            });
    }

    public void getEnemyGO(GameObject enemy)
    {
        if (enemy == null)
            return;
        enemyGO = enemy;
    }


}
