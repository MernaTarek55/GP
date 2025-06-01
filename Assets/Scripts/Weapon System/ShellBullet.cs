using UnityEngine;

public class ShellBullet : Projectile
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
                Destroy(gameObject);
            }
        }
        // Add hit effects, particles, sound etc. here

        //Destroy(gameObject);
    }
}
