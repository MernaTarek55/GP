using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Enemy Data", order = 51)]
public class EnemyData : ScriptableObject
{
    // Types of Enemies 
    [Serializable]
    public enum EnemyType
    {
        Turret = 0
       , ballDroid = 1 // ballDroid is a sphere attacking the player by exploding when he reaches his position
       , Humanoid = 2
       , LavaRobot = 3
        , LavaRobotTypeB = 4
            , Beyblade = 5
    }
    // Grouping Enemies 
    [Serializable]
    public enum EnemyGroup
    {
      Chaser = 0
      ,Shooter = 1
    }

    public EnemyGroup enemyGroup;
    public EnemyType enemyType;
    public float movementSpeed;
    public int DetectionRange;
    public int damage;
    public GameObject EnemyPrefab;
    public GameObject bulletPrefab;
    public float shootCooldown = 1f;
}