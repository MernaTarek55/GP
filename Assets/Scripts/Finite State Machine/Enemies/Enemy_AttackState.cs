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
    private Enemy enemy;
    private bool hasExploded = false; // to instantiate one explosion when the enemy explodes
    private float _lastShootTime;

    public Enemy_AttackState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;
        TryGetComponents(enemyGO);

    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();
        if (enemyData.enemyType == EnemyData.EnemyType.Turret)
        {
            Debug.Log("Turret Attack");
            if (playerGO.GetComponentInChildren<InvisibilitySkill>().isInvisible)
            {
                Debug.Log("Player is invisible");
                stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO));
                return;
            }

            RotateTowardPlayer();
            Shoot();
        }
        else if (enemyData.enemyType == EnemyData.EnemyType.ballDroid)
        {
            Debug.Log("BallDroid Attack");
            Debug.Log(playerGO.GetComponentInChildren<InvisibilitySkill>().isInvisible);
            if (playerGO.GetComponentInChildren<InvisibilitySkill>().isInvisible)
            {
                Debug.Log("Player is invisible, ball droid does nothing.");
                return;
            }
            if (!hasExploded)
            {
                ExplodingBall();
            }
        }
        else if (enemyData.enemyType == EnemyData.EnemyType.LavaRobot)
        {
            ShootLava();
        }
    }

    public override void Exit()
    {

        base.Exit();
        enemyAgent.isStopped = false;
        
    }
    private void TryGetComponents(GameObject enemyGO)
    {
        if (enemyGO.TryGetComponent(out MeshRenderer mr)) enemyMR = mr;
        else Debug.LogWarning("Mesh Renderer not found");
        if (enemyGO.TryGetComponent(out Enemy enemy)) this.enemy = enemy;
        else Debug.LogWarning("Enemy script not found");
   
        if (enemyGO.TryGetComponent(out NavMeshAgent eNav)) enemyAgent = eNav;
        else Debug.LogWarning("Nav mesh not found");
}

    //protected override void UpdateTurret()
    //{
    //    Debug.Log("Turret Attack");
    //    if (playerGO.GetComponent<InvisibilitySkill>().isInvisible)
    //    {
    //        Debug.Log("Player is invisible");
    //        stateMachine.ChangeState(new Enemy_IdleState(stateMachine, "Idle", enemyData, enemyGO));
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
        ParticleSystem enemyPSClone = GameObject.Instantiate(enemyPS, enemyGO.transform.position, enemyGO.transform.rotation);
        enemyPSClone.Play();

        enemyMR.enabled = false;
        enemy.Die();
        //GameObject.Destroy(enemyGO/*, enemyPSClone.main.duration*/);

        hasExploded = true;
        if (playerGO.GetComponentInChildren<HealthComponent>()) { playerGO.GetComponentInChildren<HealthComponent>().TakeDamage(10f); Debug.Log("Player took damage"); }
        else Debug.LogWarning("Health Component not found");
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
        if (playerGO.GetComponentInChildren<InvisibilitySkill>().isInvisible)
        {
            Debug.Log("Player is invisible, ball droid does nothing.");
            return;
        }
        // Check cooldown
        if (Time.time - _lastShootTime < enemyData.shootCooldown)
            return;

        // Check if we have valid references
        if (enemyData.bulletPrefab == null || firePoint == null)
            return;

        // Try to get a projectile from the pool
        if (LavaProjectilePool.Instance.TryGetProjectile(out GameObject projectile))
        {
            // Set up the projectile
            projectile.transform.position = firePoint.transform.position;
            projectile.transform.rotation = firePoint.transform.rotation;

            // Assign the LavaRobot reference (critical!)
            LavaProjectile lavaProjectile = projectile.GetComponent<LavaProjectile>();
            if (lavaProjectile != null)
            {
                lavaProjectile.LavaRobot = enemyGO; // or whatever reference you need
            }

            projectile.SetActive(true);
            _lastShootTime = Time.time; // Update last shoot time
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
}