using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> availableItems;

    //TODO
    [SerializeField]PlayerInventory playerInventory;

    void Awake()
    {
        availableItems = Resources.LoadAll<ShopItem>("ShopItems").ToList();
    }
    private void Start()
    {
        DisplayShopUI();
    }

    void DisplayShopUI()
    {
        //TODO
    }
    public void Buy(ShopItem item)
    {
        int cost = item.GetCost(playerInventory);
        if (playerInventory.credits >= cost)
        {
            item.OnPurchase(playerInventory);
            playerInventory.credits -= cost;
        }
    }
}
