using UnityEngine;
using DG.Tweening;
using System;

public class Enemy_IdleState : EntityState
{
    EnemyData _enemyData;
    bool _isRotating = false;

    public Enemy_IdleState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO)
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
                case 0: // Turret
                    Debug.Log("Turret");
                    if (!_isRotating)
                    {
                        RotateWithTween(new Vector3(-90, 0, 160), new Vector3(-90, 0, 0), 4f, 4f, RotateMode.Fast);
                    }
                    break;

                case (EnemyData.EnemyType)1: // Ball Droid
                    Debug.Log("ballDroid");

                    // Continuous 360 degree rotation around Y axis

                    RotateBall(new Vector3(360, 0, 0), 1f, RotateMode.WorldAxisAdd);
                    //RotateWithTween(new Vector3(0, 0, 0), new Vector3(360, 0, 0), 2f, 2f, RotateMode.WorldAxisAdd);
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


    private void RotateWithTween(Vector3 startRotation, Vector3 endRotation, float startDuration, float endDuration, RotateMode rotateMode)
    {
        _isRotating = true;



        Transform turret = enemyGO.transform;

        turret.DORotate(startRotation, startDuration, rotateMode)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // Wait 1 second, then rotate back
                DOVirtual.DelayedCall(0f, () =>
                {
                    turret.DORotate(endRotation, endDuration)
                          .SetEase(Ease.Linear)
                          .OnComplete(() =>
                          {
                              _isRotating = false;
                          });
                });
            });
    }

    private void RotateBall(Vector3 startRotation, float startDuration, RotateMode rotateMode)
    {

        enemyGO.transform.DORotate(startRotation, startDuration, rotateMode)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }






}