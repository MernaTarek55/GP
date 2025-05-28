using UnityEngine;
using DG.Tweening;

public class Enemy_IdleState : EntityState
{
    private bool _isRotating = false;
    private Tween[] _currentTween;

    public Enemy_IdleState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        _currentTween = new Tween[2];
    }

    public override void Update()
    {
        base.Update();
        if (enemyData.enemyType == EnemyData.EnemyType.Turret)
            if (!_isRotating)
            {
                RotateWithTween(new Vector3(0, 160, 0), new Vector3(0, 0, 0), 4f, 4f, RotateMode.Fast);
            }
            else if (enemyData.enemyType == EnemyData.EnemyType.ballDroid)
                RotateBall(new Vector3(360, 0, 0), 1f, RotateMode.WorldAxisAdd);
    }

    public override void Exit()
    {
        base.Exit();
        _isRotating = false;
        StopRotation();
    }

    //protected override void UpdateTurret()
    //{
    //    Debug.Log("Turret Idle");
    //    if (!_isRotating)
    //    {
    //        RotateWithTween(new Vector3(0, 160, 0), new Vector3(0, 0, 0), 4f, 4f, RotateMode.Fast);
    //    }
    //}

    //protected override void UpdateBallDroid()
    //{
    //    Debug.Log("BallDroid Idle");
    //    RotateBall(new Vector3(360, 0, 0), 1f, RotateMode.WorldAxisAdd);
    //}

    //protected override void UpdateHumanoid()
    //{
    //    Debug.Log("Humanoid Idle");
    //    // Implement humanoid idle animations here
    //}

    private void RotateWithTween(Vector3 startRotation, Vector3 endRotation, float startDuration, float endDuration, RotateMode rotateMode)
    {
        _isRotating = true;
        Transform turret = enemyGO.transform;

        _currentTween[0] = turret.DORotate(startRotation, startDuration, rotateMode)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _currentTween[1] = turret.DORotate(endRotation, endDuration, rotateMode)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => _isRotating = false);
            });
    }

    private void RotateBall(Vector3 rotation, float duration, RotateMode rotateMode)
    {
        enemyGO.transform.DORotate(rotation, duration, rotateMode)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void StopRotation()
    {
        if (_currentTween == null) return;

        foreach (var tween in _currentTween)
        {
            if (tween != null && tween.IsActive())
            {
                tween.Kill();
            }
        }
    }
}