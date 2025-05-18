using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;
    private bool isDead;

    private void Awake()
    {
        isDead = false;
        maxHealth = 100;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        //if (currentHealth <= 0)
        //{
        //    Die();
        //}
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        currentHealth -= damage;

        Debug.Log("Take Damageeeeeeeeeeeeeeee, health: " + currentHealth);
    }

    public bool IsDead() 
    {
        return isDead;
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("brakh, I think I am no longer alive");
    }
}
