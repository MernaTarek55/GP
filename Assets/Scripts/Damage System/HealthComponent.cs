using UnityEngine;
using System;

public class HealthComponent : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;
    private bool isDead;

    public event Action OnDeath;

    private void Awake()
    {
        isDead = false;
        maxHealth = 100;
        currentHealth = maxHealth;
    }

    void Update()
    {
        
        // Check if health has reached zero or below
        //if (currentHealth <= 0 && !isDead)
        //{
        //    Die();
        //}
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

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple death calls

        isDead = true;

        if(OnDeath != null)OnDeath();

    }
}