using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_AttackState : EntityState
{
    private GameObject playerGO;
    private GameObject firePoint;
    private ParticleSystem enemyPS;  // particle system for enemy ball explosion
    private MeshRenderer enemyMR;  // to disable enemy ball renderer when it explodes
    private bool hasExploded = false; // to instantiate one explosion when the enemy explodes
    private float _lastShootTime;

    public Enemy_AttackState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;
        if (enemyGO.TryGetComponent(out MeshRenderer mr)) enemyMR = mr;
        else Debug.LogWarning("Mesh Renderer not found");



    }

    protected override void UpdateTurret()
    {
        Debug.Log("Turret Attack");
        InvisibilitySkill invisibilitySkill = playerGO.GetComponent<InvisibilitySkill>();
        if (invisibilitySkill != null && invisibilitySkill.isInvisible)
        {
            Debug.Log("Player is invisible, turret does nothing.");
            stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO));
            return;
        }
        RotateTowardPlayer();
        Shoot();
    }

    protected override void UpdateBallDroid()
    {
        Debug.Log("BallDroid Attack");
        InvisibilitySkill invisibilitySkill = playerGO.GetComponent<InvisibilitySkill>();
        if (invisibilitySkill != null && invisibilitySkill.isInvisible)
        {
            Debug.Log("Player is invisible, ball droid does nothing.");
            return;
        }
        if (!hasExploded)
        {
            ParticleSystem enemyPSClone = GameObject.Instantiate(enemyPS, enemyGO.transform.position, enemyGO.transform.rotation);
            enemyPSClone.Play();
        
            enemyMR.enabled = false;
            GameObject.Destroy(enemyGO/*, enemyPSClone.main.duration*/);

            hasExploded = true;
            if(playerGO.TryGetComponent(out HealthComponent playerHealth)){ playerHealth.TakeDamage(10f); Debug.Log("Player took damage");}
            else Debug.LogWarning("Health Component not found");
        }

    }

    protected override void UpdateHumanoid()
    {
        Debug.Log("Humanoid Attack");
        // Humanoid-specific attack logic
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
            GameObject bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = firePoint.transform.position;
            bullet.transform.rotation = firePoint.transform.rotation;
            bullet.SetActive(true);
        }

        _lastShootTime = Time.time;
    }

    public void getfirePos(GameObject firPosition)
    {
        firePoint = firPosition;
    }

    public void getParticleSystem(ParticleSystem _enemyPS)
    {
        enemyPS = _enemyPS;
    }
}