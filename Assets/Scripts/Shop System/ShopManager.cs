using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Singelton { get; private set; }
    public PlayerInventory playerInventory { get; private set; }

    private List<ShopItem> availableItems;


    private void Awake()
    {
        if (Singelton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singelton = this;
        availableItems = Resources.LoadAll<ShopItem>("ShopItems").ToList();

        //TODO: remove this 
        //GameObject player = GameObject.FindWithTag("Player");
        //playerInventory = player.GetComponent<PlayerInventoryHolder>()?.Inventory;
    }
    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in scene!");
            return;
        }

        var holder = player.GetComponent<PlayerInventoryHolder>();
        if (holder == null)
        {
            Debug.LogError("PlayerInventoryHolder not found!");
            return;
        }

        if (holder.Inventory == null)
        {
            Debug.LogError("PlayerInventory is null!");
            return;
        }

        playerInventory = holder.Inventory;
    }
    public bool Buy(ShopItem item)
    {
        int cost = 0;

        if (item != null)
        {
            if (item is WeaponUpgradeItem upgradeItem)
            {
                if (playerInventory != null)
                {
                    cost = upgradeItem.GetLevelCost(playerInventory);
                }
                else
                {
                    Debug.LogError("PlayerInventory is null in ShopManager.Buy()");
                }
            }
            else
            {
                cost = item.GetCost();
            }
        }
        else
        {
            Debug.LogError("Tried to buy a null item in ShopManager.Buy()");
        }
        if (cost <= 0)
        {
            Debug.LogWarning($"Item {item.name} has a cost of {cost}, cannot be purchased.");
            return false;
        }
        if (playerInventory.Credits >= cost)
        {
            item.OnPurchase(playerInventory);
            playerInventory.Credits -= cost;

            return true;
        }
        return false;
    }
}
