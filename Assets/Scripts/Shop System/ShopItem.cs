using UnityEngine;

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
        {
            return -1; // Maxed or invalid
        }

        float normalizedLevel = (float)(level - 1) / (statData.maxLevel - 1);
        float levelCost = costPerLevel.Evaluate(normalizedLevel);
        int finalCost = Mathf.RoundToInt(baseCost * (1 + levelCost));

        return finalCost;
    }
}
[CreateAssetMenu(menuName = "Shop/Player Skill Item")]
public class PlayerSkillItem : ShopItem
{
    public PlayerSkillsStats skill;
    public float amountToIncrease = 1f; // Add this field to configure increase amount

    public override void OnPurchase(PlayerInventory inventory)
    {
        float currentValue = inventory.getPlayerStat(skill);
        inventory.SetPlayerStat(skill, currentValue + amountToIncrease);

        // Find the player's InvisibilitySkill and update its stats
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var skillComponent = player.GetComponent<InvisibilitySkill>();
            if (skillComponent != null)
            {
                skillComponent.UpdateStatsFromInventory();
            }
        }
        inventory.UpgradePlayerStat(skill)
    }
}
[CreateAssetMenu(menuName = "Shop/Health Item")]
public class HealthItem : ShopItem
{
    public int healthIncrease;

    public override void OnPurchase(PlayerInventory inventory)
    {
        // Assuming MaxHealth is tracked in playerStats
        float currentHealth = inventory.getPlayerStat(PlayerSkillsStats.MaxHealth);
        inventory.SetPlayerStat(PlayerSkillsStats.MaxHealth, currentHealth + healthIncrease);

        // You may also want to immediately heal the player
        // This would require access to the Player component

    }
}
