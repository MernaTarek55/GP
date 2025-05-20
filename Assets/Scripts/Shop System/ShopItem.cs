using UnityEngine;
using UnityEngine.UI;

public abstract class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [SerializeField]
    protected int baseCost;
    public abstract void OnPurchase(PlayerInventory inventory);
    public virtual int GetCost(PlayerInventory inventory)
    {
        return baseCost; // Override if cost changes per level
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
            inventory.AddWeapon(weaponType);
            inventory.credits -= baseCost;
        }
    }
}
[CreateAssetMenu(menuName = "Shop/Weapon Upgrade Item")]
public class WeaponUpgradeItem : ShopItem
{
    public WeaponType weaponType;
    public UpgradableStatType statToUpgrade;

    public override void OnPurchase(PlayerInventory inventory)
    {
        var upgradeState = inventory.GetUpgradeState(weaponType);
        upgradeState.UpgradeLevel(statToUpgrade);
        inventory.credits -= GetCost(inventory);
    }
    public override int GetCost(PlayerInventory inventory)
    {
        var level = inventory.GetUpgradeState(weaponType).GetLevel(statToUpgrade);
        var weaponData = WeaponDatabase.GetWeaponData(weaponType);

        var statData = weaponData.upgradableStats.Find(s => s.statType == statToUpgrade);
        if (statData == null || level >= statData.maxLevel)
            return -1; // Maxed or invalid

        return statData.GetCost(level);
    }
}
[CreateAssetMenu(menuName = "Shop/Player Skill Item")]
public class PlayerSkillItem : ShopItem
{
    public PlayerSkillsStats skill;
    public override void OnPurchase(PlayerInventory inventory)
    {
        //TODO
    }
}
[CreateAssetMenu(menuName = "Shop/Health Item")]
public class HealthItem : ShopItem
{
    public int healthIncrease;

    public override void OnPurchase(PlayerInventory inventory)
    {
        //TODO
    }
}
