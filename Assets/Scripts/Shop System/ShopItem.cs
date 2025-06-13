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

[CreateAssetMenu(menuName = "Shop/Weapon Item")]
public class WeaponItem : ShopItem
{
    public WeaponType weaponType;
    

    public override void OnPurchase(PlayerInventory inventory)
    {
        if (!inventory.HasWeapon(weaponType))
        {
            Debug.LogWarning("Weapon added" + weaponType);
            inventory.AddWeapon(weaponType);
            inventory.inventorySaveData.printData();
            // Notify weapon switch system
            var weaponSwitch = FindObjectOfType<WeaponSwitch>();
            weaponSwitch?.OnWeaponPurchased(weaponType);
        }
            isOwned = true;
    }
}
