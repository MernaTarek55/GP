using UnityEngine;

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
