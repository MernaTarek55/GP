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
        GameObject player = GameObject.FindWithTag("Player");
        playerInventory = player.GetComponent<PlayerInventoryHolder>()?.Inventory;
    }
    private void Start()
    {
    }
    public bool Buy(ShopItem item)
    {
        int cost = item is WeaponUpgradeItem upgradeItem ? upgradeItem.GetLevelCost(playerInventory) : item.GetCost();
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
