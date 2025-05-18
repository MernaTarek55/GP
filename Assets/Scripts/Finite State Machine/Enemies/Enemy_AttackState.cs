using System.Collections;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

        if (_enemyData.bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = GameObject.Instantiate(_enemyData.bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(enemyGO.transform.forward * 500f); 
            }
        }

        _lastShootTime = Time.time;
    }
    public void getPlayer(GameObject player)
    {
        playerGO = player;
    }
    public void getfirePos(GameObject firPosition)
    {
        firePoint = firPosition;
    }

   



}
