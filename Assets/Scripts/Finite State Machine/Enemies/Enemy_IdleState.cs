using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class Enemy_IdleState : EntityState
{
    private bool _isRotating = false;
    private Tween[] _currentTween;
    private GameObject playerGO;
    private InvisibilitySkill invisibilitySkill;

    public Enemy_IdleState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;
        TryGetComponents(playerGO);
        _currentTween = new Tween[2];
    }

    public override void Update()
    {
        base.Update();
        switch (enemyData.enemyType)
        {
            case EnemyData.EnemyType.Turret:
                if (!_isRotating)
                {
                    Debug.LogWarning("Turret Idle");
                    RotateWithTween(new Vector3(0, -160, 0), new Vector3(0, 0, 0), 4f, 4f, RotateMode.Fast);
                }
                break; 

            case EnemyData.EnemyType.ballDroid: 
                RotateOnSelf(new Vector3(360, 0, 0), 1f, RotateMode.WorldAxisAdd);
                break; 
            case EnemyData.EnemyType.Beyblade:
                RotateOnSelf(new Vector3(0, 360, 0), 1f, RotateMode.WorldAxisAdd);
                break;
            default:
                break;
        }
    }

    public override void Exit()
    {
        base.Exit();
        _isRotating = false;
        StopRotation();
        //if (enemyData.enemyType == EnemyData.EnemyType.ballDroid){}
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
    //    RotateOnSelf(new Vector3(360, 0, 0), 1f, RotateMode.WorldAxisAdd);
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

    private void RotateOnSelf(Vector3 rotation, float duration, RotateMode rotateMode)
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
    public override void CheckStateTransitions(float distanceToPlayer)
    {
        if (!invisibilitySkill.isInvisible)
        {

            if (distanceToPlayer > enemyData.DetectionRange && enemyData.enemyGroup == EnemyData.EnemyGroup.Shooter && enemyData.enemyType != EnemyData.EnemyType.Turret)
            {
                stateMachine.ChangeState(new Enemy_PatrolState(stateMachine, "Patrol", enemyData, enemyGO, playerGO));
            }

            if (distanceToPlayer <= enemyData.DetectionRange)
            {

                if (enemyData.enemyGroup == EnemyData.EnemyGroup.Chaser)
                {
                    stateMachine.ChangeState(new Enemy_ChaseState(stateMachine, "Chase", enemyData, enemyGO, playerGO, enemy));
                }
                else if (enemyData.enemyType != EnemyData.EnemyType.LavaRobot &&
                         enemyData.enemyType != EnemyData.EnemyType.LavaRobotTypeB)
                {
                    stateMachine.ChangeState(new Enemy_AttackState(stateMachine, "Attack", enemyData, enemyGO, playerGO));
                }
            }
        }
        else
        {
            if (enemyData.enemyGroup == EnemyData.EnemyGroup.Shooter && enemyData.enemyType != EnemyData.EnemyType.Turret)
            {
                stateMachine.ChangeState(new Enemy_PatrolState(stateMachine, "Patrol", enemyData, enemyGO, playerGO));
            }
        }
    }

    private void TryGetComponents(GameObject entityGO)
    {
        if (entityGO.CompareTag("Player"))
        {
          
            if (entityGO.TryGetComponent(out InvisibilitySkill invisibilitySkill)) this.invisibilitySkill = invisibilitySkill;
            else Debug.LogWarning("invisibilitySkill not found");

        }
      
    }
}
