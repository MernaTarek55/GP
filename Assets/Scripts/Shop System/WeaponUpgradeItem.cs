using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Weapon Upgrade Item")]
public class WeaponUpgradeItem : ShopItem
{
    public WeaponType weaponType;
    public UpgradableStatType statToUpgrade;

    public AnimationCurve costPerLevel;
    public int maxLvl;


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
        {
            return -1; // Maxed or invalid
        }

        float normalizedLevel = (float)(level - 1) / (statData.maxLevel - 1);
        float levelCost = costPerLevel.Evaluate(normalizedLevel);
        int finalCost = Mathf.RoundToInt(baseCost * (1 + levelCost));

        return finalCost;
    }
}