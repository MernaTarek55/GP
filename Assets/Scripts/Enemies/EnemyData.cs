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
    }
    public EnemyType enemyType;
    [SerializeField] float movementSpeed;
    [SerializeField] int DetectionRange;
    [SerializeField] int damage;
    [SerializeField] GameObject EnemyPrefab;

}