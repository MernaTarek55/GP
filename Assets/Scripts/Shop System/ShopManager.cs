using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Singelton { get; private set; }
    public PlayerInventory playerInventory { get; private set; }

    [SerializeField] private List<ShopItem> availableItems;

    private void Awake()
    {
        if (Singelton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singelton = this;
        if(availableItems.Count < 1)
            availableItems = Resources.LoadAll<ShopItem>("ShopItems").ToList();

        //TODO: remove this 
        //GameObject player = GameObject.FindWithTag("Player");
        playerInventory = SaveManager.Singleton.playerInventoryHolder.Inventory;
    }
    private void Start()
    {
        
        
        if(!GameStartType.IsNewGame)
        {
            foreach (var item in availableItems)
            {
                if (item is WeaponUpgradeItem upgradeItem)
                {
                    if (playerInventory.GetUpgradeState(upgradeItem.weaponType).GetLevel(upgradeItem.statToUpgrade) >= upgradeItem.maxLvl)
                    {
                        upgradeItem.isOwned = true;
                    }
                    else
                    {
                        upgradeItem.isOwned = false;
                    }
                }
                else if (item is WeaponItem weaponItem)
                {
                    if(playerInventory.HasWeapon(weaponItem.weaponType))
                    {
                        weaponItem.isOwned = true;
                    }
                    else
                    {
                        weaponItem.isOwned = false;
                    }
                }
                else if(item is PlayerSkillItem playerSkillItem)
                { 
                    if (playerInventory.getPlayerStat(playerSkillItem.skill) <= playerSkillItem.maxVal)
                    {
                        playerSkillItem.isOwned = true;
                    }
                    else
                    {
                        playerSkillItem.isOwned = false;
                    }
                }
                
            }
        }
        else
        {
            Debug.LogWarning("Initializing shop for new game...");
            // For new game, ensure all items are not owned
            foreach (var item in availableItems)
            {
                item.isOwned = false;
            }
        }
    }
    public bool Buy(ShopItem item)
    {
        int cost = 0;

        if (item != null)
        {
            if (item.isOwned)
            {
                Debug.LogWarning($"Item {item.name} is already owned, cannot be purchased again.");
                return false;
            }
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
