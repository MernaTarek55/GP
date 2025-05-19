using UnityEngine;
using DG.Tweening;

public class Enemy_IdleState : EntityState
{
    EnemyData _enemyData;
    bool _isRotating = false;
    Tween[] currentTween; // Initialize with 2 elements

    public Enemy_IdleState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        _enemyData = enemyData;
        currentTween = new Tween[2];
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
                        RotateWithTween(new Vector3(0, 160, 0), new Vector3(0, 0, 0), 4f, 4f, RotateMode.Fast);
                    }
                    break;

                case (EnemyData.EnemyType)1: // Ball Droid
                    Debug.Log("ballDroid");
                    RotateBall(new Vector3(360, 0, 0), 1f, RotateMode.WorldAxisAdd);
                    break;

                case (EnemyData.EnemyType)2: // Humanoid
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

        currentTween[0] = turret.DORotate(startRotation, startDuration, rotateMode)
    .SetEase(Ease.Linear)
    .OnComplete(() =>
    {
        DOVirtual.DelayedCall(0f, () =>
        {
            currentTween[1] = turret.DORotate(endRotation, endDuration, rotateMode)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _isRotating = false;
                })
                .OnPause(() => { Debug.Log("pause"); });
        });
    }).OnPause(()=> { Debug.Log("pause"); });
    //.OnKill(() =>
    //{
    //    Debug.Log("Tween 0 killed");
    //});


    }

    private void RotateBall(Vector3 rotation, float duration, RotateMode rotateMode)
    {
        enemyGO.transform.DORotate(rotation, duration, rotateMode)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    public override void Exit()
    {
        base.Exit();
        _isRotating = false;

        StopRotation();
    }

    private void StopRotation()
    {
        if (currentTween == null || currentTween.Length == 0)
            return;

        for (int i = 0; i < currentTween.Length; i++)
        {
            if (currentTween[i] != null && currentTween[i].IsActive())
            {
                currentTween[i].Pause(); // This triggers .OnKill()
                currentTween[i] = null;
            }
        }

        _isRotating = false; // Move this AFTER killing
    }

}

