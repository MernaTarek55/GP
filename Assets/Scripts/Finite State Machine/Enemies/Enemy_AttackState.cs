using System.Collections.Generic;
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
        if (firePoint == null)
        {
            Debug.LogError("Fire position not found!");
        }

    }

    public override void Update()
    {
        base.Update();
        if (enemyData.enemyType == EnemyData.EnemyType.Turret  || enemyData.enemyType == EnemyData.EnemyType.OneArmedRobot)
        {
            Debug.Log("Turret Attack");
            if (invisibilitySkill.isInvisible)
            {
                Debug.Log("Player is invisible");
                stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
                return;
            }

            RotateTowardPlayer();
            if(enemyData.enemyType == EnemyData.EnemyType.OneArmedRobot)
            ShootRobot();
            else
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
        enemy.Die();

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
        Debug.LogWarning("LavaRobot Attack");
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
            LavaProjectile lavaProjectile = projectile.GetComponent<LavaProjectile>();
            Debug.Log("LavaProjectile: " + lavaProjectile);
            initProjectileType(lavaProjectile);

            //projectile.GetComponent<LavaProjectile>().LavaRobot = enemyGO;
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
    private void ShootRobot()
    {
        if (Time.time - _lastShootTime < enemyData.shootCooldown)
            return;

        // Get shoot position from child object
        Transform shootPos = GetChildrenWithTag(enemyGO)?.transform;
        if (shootPos == null)
        {
            Debug.LogError("Shoot position not found!");
            return;
        }

        // Check if facing player (relax angle check slightly)
        Vector3 directionToPlayer = (playerGO.transform.position - enemyGO.transform.position).normalized;
        directionToPlayer.y = 0;
        float angle = Vector3.Angle(enemyGO.transform.forward, directionToPlayer);

        if (angle > 30f) return; // Increased from 10 to 30 degrees

        // Get bullet from pool
        if (PoolManager.Instance == null)
        {
            Debug.LogError("PoolManager instance not found!");
            return;
        }

        GameObject bullet = PoolManager.Instance.GetPrefabByTag(PoolType.Bullet);
        if (bullet == null)
        {
            Debug.LogError("Bullet prefab not found in pool!");
            return;
        }

        bullet.transform.position = shootPos.position;
        bullet.transform.rotation = shootPos.rotation;
        bullet.SetActive(true);
        Debug.Log($"Shooting - Angle to player: {angle}, FirePos: {shootPos.position}");
        _lastShootTime = Time.time;
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


        
            QuadraticCurve quadraticCurve = enemyGO.GetComponentInChildren<QuadraticCurve>();

   

            lavaProjectile.SetQuadraticCurve(quadraticCurve);
            lavaProjectile.SetenemyPosition(enemyGO.transform.position);
            switch (enemyData.enemyType)
            {
            


            case EnemyData.EnemyType.LavaRobot:
                lavaProjectile.projectileType = LavaProjectile.ProjectileEnemyType.Target;  //Target shooting:
                break;
            case EnemyData.EnemyType.LavaRobotTypeB:
                lavaProjectile.projectileType = LavaProjectile.ProjectileEnemyType.Random; //Random shooting

                break;
            }
        
        

    }

    public override void CheckStateTransitions(float distanceToPlayer)
    {
        if (invisibilitySkill.isInvisible && enemyData.enemyType != EnemyData.EnemyType.LavaRobot && enemyData.enemyType != EnemyData.EnemyType.LavaRobotTypeB && enemyData.enemyType != EnemyData.EnemyType.OneArmedRobot)
        {
            Debug.Log("Player is invisible, enemy does nothing.");
            stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO, playerGO));
            return;
        }
       
            if (distanceToPlayer > enemyData.DetectionRange)
            {
                if (enemyData.enemyGroup == EnemyData.EnemyGroup.Shooter && enemyData.enemyType != EnemyData.EnemyType.Turret)
                {
                    stateMachine.ChangeState(new Enemy_PatrolState(stateMachine, "Patrol", enemyData, enemyGO, playerGO));
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
    private GameObject GetChildrenWithTag(GameObject parentObject)
    {
        if (parentObject == null) return null;

        foreach (Transform child in parentObject.transform)
        {
            if (child.CompareTag("ShootPos"))
                return child.gameObject;

            // Recursively check children if needed
            var foundInChild = GetChildrenWithTag(child.gameObject);
            if (foundInChild != null)
                return foundInChild;
        }
        return null;
    }
}