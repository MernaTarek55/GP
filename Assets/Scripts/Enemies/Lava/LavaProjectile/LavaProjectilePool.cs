using System.Collections.Generic;
using UnityEngine;

public class LavaProjectilePool : MonoBehaviour
{
    public static LavaProjectilePool Instance;

    [SerializeField] private GameObject lavaProjectilePrefab;
    [SerializeField] private int poolSize = 1; // Only need one active at a time

    private Queue<GameObject> projectilePool;
    private bool isProjectileActive = false;

    private void Awake()
    {
        Instance = this;
        projectilePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(lavaProjectilePrefab, transform);
            projectile.SetActive(false);
            projectilePool.Enqueue(projectile);
        }
    }

    public bool TryGetProjectile(out GameObject projectile)
    {
        projectile = null;

        if (isProjectileActive || projectilePool.Count == 0)
            return false;

        projectile = projectilePool.Dequeue();
        isProjectileActive = true;
        return true;
    }


    public void ReturnToPool(GameObject projectile)
    {
        projectile.SetActive(false);
        projectilePool.Enqueue(projectile);
        isProjectileActive = false;
    }
}