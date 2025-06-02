using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;
    private bool isDead;

    private void Awake()
    {
        isDead = false;


        // don't merge this change!!!!!!!
        maxHealth = 100000;
        //maxHealth = 100;


        currentHealth = maxHealth;
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
