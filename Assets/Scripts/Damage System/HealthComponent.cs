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

    public void RenewHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
    }
    public void setMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        currentHealth -= damage;

    }

    public bool IsDead()
    {
        return isDead;
    }

    private void Die()
    {
        isDead = true;
    }
}
