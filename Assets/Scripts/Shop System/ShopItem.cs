using UnityEngine;

public abstract class ShopItem : ScriptableObject
{
    public int baseCost;
    public bool isOwned = false;
    public abstract void OnPurchase(PlayerInventory inventory);
    public virtual int GetCost()
    {
        return baseCost;
    }

    // Override if cost changes per level
    public virtual int GetLevelCost(PlayerInventory inventory)
    {
        Debug.LogWarning("GetLevelCost not implemented in " + name);
        return -1;
    }

}

