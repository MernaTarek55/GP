using UnityEngine;

public class Enemy_AttackState : EntityState
{
    private GameObject playerGO;
    private GameObject firePoint;
    private ParticleSystem ballPS;
    private MeshRenderer ballMR;
    private float _lastShootTime;

    public Enemy_AttackState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.playerGO = playerGO;
        if (!enemyGO.TryGetComponent(out ballPS))
            Debug.LogWarning("No particle System");
        if (!enemyGO.TryGetComponent(out ballMR))
            Debug.LogWarning("No Mesh Renderer");
    }

    protected override void UpdateTurret()
    {
        Debug.Log("Turret Attack");
        RotateTowardPlayer();
        Shoot();
    }

    protected override void UpdateBallDroid()
    {
        Debug.Log("BallDroid Attack");
        ballPS.Play();
        ballMR.enabled = false;
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
}