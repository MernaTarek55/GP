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
        currentHealth = 100;
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

        Debug.Log("Take Damageeeeeeeeeeeeeeee, health: " + currentHealth);
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void Die()
    {
        isDead = true;
        GetComponentInParent<Enemy>()?.Die();
        Debug.Log("bruh, I think I am no longer alive");
    }
}
