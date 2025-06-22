using UnityEngine;
using System;

public class HealthComponent : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;
    private bool isDead;

    [SerializeField] private ParticleSystem HitEffect;
    public event Action OnDeath;

    private void Awake()
    {
        isDead = false;
        maxHealth = 100;
        currentHealth = maxHealth;
    }

    void Update()
    {
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
        HitEffect.Play();

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