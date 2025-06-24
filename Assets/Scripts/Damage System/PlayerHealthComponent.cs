using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealthComponent : MonoBehaviour, IDamageable
{
    private PlayerInventoryHolder inventoryHolder;

    private void Start()
    {
        inventoryHolder = SaveManager.Singleton.GetComponent<PlayerInventoryHolder>();
       
    }
    private void Update()
    {
        //TakeDamage(10);
    }
    public void TakeDamage(float damage)
    {
        if (IsDead()) return;

        inventoryHolder.Inventory.CurrentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Health: {inventoryHolder.Inventory.CurrentHealth}");

        if (inventoryHolder.Inventory.CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        inventoryHolder.Inventory.CurrentHealth += amount;
        Debug.Log($"Player healed {amount}. Health: {inventoryHolder.Inventory.CurrentHealth}");
    }

    public void RenewHealth()
    {
        inventoryHolder.Inventory.CurrentHealth = inventoryHolder.Inventory.MaxHealth;
    }

    public bool IsDead()
    {
        return inventoryHolder.Inventory.CurrentHealth <= 0;
    }

    private void Die()
    {
        Debug.Log("Player has died");
        // Add player death logic here (respawn, game over, etc.)
    }
}