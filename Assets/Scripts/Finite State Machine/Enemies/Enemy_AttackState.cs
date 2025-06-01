using System.Security.Cryptography;
﻿using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AttackState : EntityState
{
    private GameObject playerGO;
    private GameObject firePoint;
    private ParticleSystem enemyPS;  // particle system for enemy ball explosion
    private MeshRenderer enemyMR;  // to disable enemy ball renderer when it explodes
    private NavMeshAgent enemyAgent; // to let enemy patrol and chase player
    private SphereCollider sphereCollider;
    private Enemy enemy;
    private HealthComponent playerHealth;
    private InvisibilitySkill invisibilitySkill;
    private bool hasExploded = false; // to instantiate one explosion when the enemy explodes

    private float _lastShootTime;

    public Enemy_AttackState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;
        TryGetComponents(enemyGO);
        TryGetComponents(playerGO);


    }

    public override void Enter()
    {
        base.Enter();
        firePoint = enemy.getfirePos();

    }

    public override void Update()
    {
        base.Update();
        if (enemyData.enemyType == EnemyData.EnemyType.Turret)
        {
            Debug.Log("Turret Attack");
            if (invisibilitySkill.isInvisible)
            {
                Debug.Log("Player is invisible");
                stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
                return;
            }

            RotateTowardPlayer();
            Shoot();
        }
        else if (enemyData.enemyGroup == EnemyData.EnemyGroup.Chaser)
        {
            Debug.Log("BallDroid Attack");
            Debug.Log(invisibilitySkill.isInvisible);
            if (invisibilitySkill.isInvisible)
            {
                Debug.Log("Player is invisible, ball droid does nothing.");
                return;
            }
            if (!hasExploded && enemyData.enemyType == EnemyData.EnemyType.ballDroid)
            {
                ExplodingBall();
            }
            
        }
        else if (enemyData.enemyType == EnemyData.EnemyType.LavaRobot || enemyData.enemyType == EnemyData.EnemyType.LavaRobotTypeB)
        {
            ShootLava();
        }
    }

    public override void Exit()
    {

        base.Exit();


        if (enemyData.enemyType == EnemyData.EnemyType.Beyblade)
        {
            BeybladeAttack();
        }
    }
    private void TryGetComponents(GameObject entityGO)
    {
        if (entityGO.CompareTag("Player"))
        {
            if(entityGO.TryGetComponent(out HealthComponent healthComponent)) playerHealth = healthComponent;
            else Debug.LogWarning("Health Component not found");
            if (entityGO.TryGetComponent(out InvisibilitySkill invisibilitySkill)) this.invisibilitySkill = invisibilitySkill;
            else Debug.LogWarning("invisibilitySkill not found");
            
        }
        else
        {
            if (entityGO.TryGetComponent(out MeshRenderer mr)) enemyMR = mr;
            else Debug.LogWarning("Mesh Renderer not found");
            if (entityGO.TryGetComponent(out Enemy enemy)) this.enemy = enemy;
            else Debug.LogWarning("Enemy script not found");

            if (entityGO.TryGetComponent(out NavMeshAgent eNav)) enemyAgent = eNav;
            else Debug.LogWarning("Nav mesh not found");
  
            if (entityGO.TryGetComponent(out SphereCollider sphereCollider)) this.sphereCollider = sphereCollider;
            else Debug.LogWarning("Mesh Renderer not found");
        }
}

    //protected override void UpdateTurret()
    //{
    //    Debug.Log("Turret Attack");
    //    if (playerGO.GetComponent<InvisibilitySkill>().isInvisible)
    //    {
    //        Debug.Log("Player is invisible");
    //        stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, entityGO));
    //        return;
    //    }

    //    RotateTowardPlayer();
    //    Shoot();
    //}

    //protected override void UpdateBallDroid()
    //{
    //    Debug.Log("BallDroid Attack");
    //    Debug.Log(playerGO.GetComponent<InvisibilitySkill>().isInvisible);
    //    if (playerGO.GetComponent<InvisibilitySkill>().isInvisible)
    //    {
    //        Debug.Log("Player is invisible, ball droid does nothing.");
    //        return;
    //    }
    //    if (!hasExploded)
    //    {
    //        ExplodingBall();
    //    }

    //}

    private void ExplodingBall()
    {
        enemy.particleEffect.Play();

        enemyMR.enabled = false;
        sphereCollider.enabled = false;
        //enemy.Die();

        //GameObject.Destroy(entityGO/*, enemyPSClone.main.duration*/);

        hasExploded = true;
        playerHealth.TakeDamage(10f);
    }

    //protected override void UpdateHumanoid()
    //{
    //    Debug.Log("Humanoid Attack");
    //    // Humanoid-specific attack logic
    //}

    //protected override void UpdateLavaRobot()
    //{
    //    Debug.Log("LavaRobot Attack");

    //    ShootLava();
    //}

    private void ShootLava()
    {
        if (invisibilitySkill.isInvisible)
        {
            Debug.Log("Player is invisible, ball droid does nothing.");
            return;
        }
        // Check cooldown
        if (Time.time - _lastShootTime < enemyData.shootCooldown)
        return; 
        
        if(firePoint == null)  /*getfirePos(firePoint);*/ return;
        



        // Try to get a projectile from the pool
        if (PoolManager.Instance.GetPrefabByTag(PoolType.LavaProjectile))
        {
            // Set up the projectile
            GameObject projectile = PoolManager.Instance.GetPrefabByTag(PoolType.LavaProjectile);
            initProjectileType(projectile.GetComponent<LavaProjectile>());
            projectile.GetComponent<LavaProjectile>().LavaRobot = enemyGO;
            if (enemyData.bulletPrefab != null && firePoint != null)
            {
                // Set up the projectile
                //projectile.transform.position = firePoint.transform.position;
                //projectile.transform.rotation = firePoint.transform.rotation;

                // Assign the LavaRobot reference (critical!)
               
                projectile.SetActive(true);
                _lastShootTime = Time.time; // Update last shoot time
            }
        }
    }

    private void RotateTowardPlayer()
    {
        Vector3 direction = (playerGO.transform.position - enemyGO.transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            enemyGO.transform.rotation = Quaternion.Slerp(enemyGO.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void Shoot()
    {
        if (Time.time - _lastShootTime < enemyData.shootCooldown)
            return;

        Vector3 directionToPlayer = (playerGO.transform.position - enemyGO.transform.position).normalized;
        directionToPlayer.y = 0;
        float angle = Vector3.Angle(enemyGO.transform.forward, directionToPlayer);

        if (angle > 10f) return;

        if (enemyData.bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = PoolManager.Instance.GetPrefabByTag(PoolType.Bullet);
            bullet.transform.position = firePoint.transform.position;
            bullet.transform.rotation = firePoint.transform.rotation;
            bullet.SetActive(true);
            Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        }

        _lastShootTime = Time.time;
    }

    private void BeybladeAttack()
    {
        RotateOnSelf(new Vector3(0, 540, 0), 1f, RotateMode.WorldAxisAdd);
        playerHealth.TakeDamage(1f); 
    }

  

    private void RotateOnSelf(Vector3 rotation, float duration, RotateMode rotateMode)
    {
        enemyGO.transform.DORotate(rotation, duration, rotateMode)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    public void initProjectileType(LavaProjectile lavaProjectile)
    {
        

            switch (enemyData.enemyType)
            {
            


            case EnemyData.EnemyType.LavaRobot:
                lavaProjectile.projectileType = LavaProjectile.ProjectileEnemyType.Target;  //Target shooting:
                Debug.Log("Enemy Target");
                break;
            case EnemyData.EnemyType.LavaRobotTypeB:
                lavaProjectile.projectileType = LavaProjectile.ProjectileEnemyType.Random; //Random shooting
                Debug.Log("Enemy Random");

                break;
            }

        

    }

    public override void CheckStateTransitions(float distanceToPlayer)
    {
        if (invisibilitySkill.isInvisible)
        {
            stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
            return;
        }

        if (distanceToPlayer > enemyData.DetectionRange)
        {
            if (enemyData.enemyType == EnemyData.EnemyType.LavaRobot ||
                enemyData.enemyType == EnemyData.EnemyType.LavaRobotTypeB)
            {
                stateMachine.ChangeState(new Enemy_PatrolState(stateMachine, "Patrol", enemyData, enemyGO,playerGO));
            }
            else
            {
                stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
            }
        }
        else if (enemyData.enemyGroup == EnemyData.EnemyGroup.Chaser && distanceToPlayer > 2f)
        {
            stateMachine.ChangeState(new Enemy_ChaseState(stateMachine, "Chase", enemyData, enemyGO, playerGO, enemy));
        }
    }
}