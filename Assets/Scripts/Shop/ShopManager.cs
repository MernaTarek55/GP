using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> inventory;
    public int playerCredits;

    public bool BuyItem(ShopItem item)
    {
        if (playerCredits >= item.cost)
        {
            Debug.Log($"Bought {item.itemName} for {item.cost} credits.");
            return true;
        }
        else
        {
            Debug.Log("Not enough credits!");
            return false;
        }
    }

    public void DisplayItems()
    {
        // Integrate with UI to display items
    }
}
