using System.Collections.Generic;
using UnityEngine;

public class LavaProjectilePool : MonoBehaviour
{
    public static LavaProjectilePool Instance;

    [SerializeField] private GameObject lavaProjectilePrefab;
    [SerializeField] private int poolSize = 5; // Increased pool size for multiple projectiles
    [SerializeField] private int maxActiveProjectiles = 3; // Maximum allowed active projectiles

    private Queue<GameObject> projectilePool;
    private List<GameObject> activeProjectiles = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

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

        // Check if we've reached max active projectiles
        if (activeProjectiles.Count >= maxActiveProjectiles || projectilePool.Count == 0)
        {
            return false;
        }

        projectile = projectilePool.Dequeue();
        projectile.SetActive(true);
        activeProjectiles.Add(projectile);
        return true;
    }

    public void ReturnToPool(GameObject projectile)
    {
        if (activeProjectiles.Contains(projectile))
        {
            projectile.SetActive(false);
            projectilePool.Enqueue(projectile);
            activeProjectiles.Remove(projectile);
        }
        else
        {
            Debug.LogWarning("Trying to return a projectile that wasn't active!");
        }
    }

    public int GetActiveProjectileCount()
    {
        return activeProjectiles.Count;
    }
}