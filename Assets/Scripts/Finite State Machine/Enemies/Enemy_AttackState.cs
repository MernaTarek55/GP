using System.Collections;
using UnityEditor;
using UnityEngine;

public class Enemy_AttackState : EntityState
{
    EnemyData _enemyData;
    GameObject playerGO;
        GameObject firePoint;
    ParticleSystem ballPS;
    MeshRenderer ballMR;
    float _lastShootTime;
    public Enemy_AttackState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO, GameObject playerGO)

    
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        _enemyData = enemyData;
        this.playerGO = playerGO;
        if (!enemyGO.TryGetComponent(out ballPS))
                Debug.LogWarning("No particle System");
        if (!enemyGO.TryGetComponent(out ballMR))
            Debug.LogWarning("No Mesh Renderer");


    }

    public override void Update()
    {
        base.Update();
        Debug.Log("I am in enemy");
        if (_enemyData != null)
        {
            switch (_enemyData.enemyType)
            {
                case 0:
                    Debug.Log("Turret");
                    
                        RotateTowardPlayer();
                        Shoot();
                    
                    break;

                case (EnemyData.EnemyType)1:
                    Debug.Log("ballDroid");
                   

                        ballPS.Play();
                        ballMR.enabled = false;



                    break;

                case (EnemyData.EnemyType)2:
                    Debug.Log("Humanoid");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Ana Null");
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
        if (Time.time - _lastShootTime < _enemyData.shootCooldown)
            return;

        // Check if turret is facing the player
        Vector3 directionToPlayer = (playerGO.transform.position - enemyGO.transform.position).normalized;
        directionToPlayer.y = 0;
        float angle = Vector3.Angle(enemyGO.transform.forward, directionToPlayer);

        if (angle > 10f) // If angle is more than 10 degrees, don't shoot yet
            return;

        if (_enemyData.bulletPrefab != null && firePoint != null)
        {
            //GameObject bullet = GameObject.Instantiate(_enemyData.bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
            //Rigidbody rb = bullet.GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    rb.AddForce(enemyGO.transform.forward * 500f);
            //}
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
