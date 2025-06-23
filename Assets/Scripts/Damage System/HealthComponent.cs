using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour, IDamageable
{
    private float currentHealth;
    private float maxHealth;
    private bool isDead;
    //textmeshpro
    public TextMeshProUGUI hp;
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

        hp.text = "health: " + currentHealth;
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
