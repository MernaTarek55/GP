using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Enemy Data", order = 51)]
public class EnemyData : ScriptableObject
{
    // Types of Enemies 
    public enum EnemyType
    {
         Turret = 0
       , ballDroid = 1 // ballDroid is a sphere attacking the player by exploding when he reaches his position
       , Humanoid = 2
    }
    [SerializeField]
    private EnemyType enemyClass;
    [SerializeField] private float movementSpeed;
    [SerializeField] private int DetectionRange;
    [SerializeField] private int damage;
    [SerializeField] private GameObject EnemyPrefab;

}