using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Singelton { get; private set; }
    public PlayerInventory playerInventory { get; private set; }

    List<ShopItem> availableItems;


    void Awake()
    {
        if (Singelton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singelton = this;
        availableItems = Resources.LoadAll<ShopItem>("ShopItems").ToList();

        //TODO: remove this
        var player = GameObject.FindWithTag("Player");
        playerInventory = player.GetComponent<PlayerInventoryHolder>()?.Inventory;
    }
    private void Start()
    {
    }

    void DisplayShopUI()
    {
        //TODO
    }
    public bool Buy(ShopItem item)
    {
        int cost;
        if (item is WeaponUpgradeItem upgradeItem)
            cost = upgradeItem.GetLevelCost(playerInventory);
        else
            cost = item.GetCost();
        if (playerInventory.credits >= cost)
        {
            item.OnPurchase(playerInventory);
            playerInventory.credits -= cost;
            return true;
        }
        return false;
    }
}
