using UnityEngine;
using UnityEngine.UI;

public abstract class ShopItem : ScriptableObject
{
    [SerializeField]
    protected int baseCost;
    public abstract void OnPurchase(PlayerInventory inventory);
    public virtual int GetCost()
    {
        return baseCost;
    }

    // Override if cost changes per level
    public virtual int GetLevelCost(PlayerInventory inventory)
    {
        Debug.LogWarning("GetLevelCost not implemented in " + this.name);
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
            inventory.AddWeapon(weaponType);
        }
    }
}
[CreateAssetMenu(menuName = "Shop/Weapon Upgrade Item")]
public class WeaponUpgradeItem : ShopItem
{
    public WeaponType weaponType;
    public UpgradableStatType statToUpgrade;

    public AnimationCurve costPerLevel;

    public override void OnPurchase(PlayerInventory inventory)
    {
        WeaponUpgradeState upgradeState = inventory.GetUpgradeState(weaponType);
        upgradeState.UpgradeLevel(statToUpgrade);
    }
    public override int GetLevelCost(PlayerInventory inventory)
    {
        int level = inventory.GetUpgradeState(weaponType).GetLevel(statToUpgrade);
        WeaponData weaponData = WeaponDatabase.GetWeaponData(weaponType);
        UpgradableStat statData = weaponData.upgradableStats.Find(s => s.statType == statToUpgrade);

        if (statData == null || level >= statData.maxLevel)
            return -1; // Maxed or invalid

        return Mathf.RoundToInt(costPerLevel.Evaluate(level));
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
