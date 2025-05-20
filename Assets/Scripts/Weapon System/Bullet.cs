using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] private float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    protected override void OnHit(Collider other)
    {
        // Damage enemy
        if (other.CompareTag("Enemy"))
        {
            HealthComponent enemyHealth = other.GetComponent<HealthComponent>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        // Add hit effects, particles, sound etc. here

        Destroy(gameObject);
    }
}
