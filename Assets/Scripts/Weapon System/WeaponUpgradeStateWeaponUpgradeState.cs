using System.Collections.Generic;

[System.Serializable]
public class WeaponUpgradeState
{
    private Dictionary<UpgradableStatType, int> statLevels = new();

    public int GetLevel(UpgradableStatType statType)
    {
        return statLevels.TryGetValue(statType, out int level) ? level : 0;
    }
    public int SetLevel(UpgradableStatType statType, int level)
    {
        if (level < 0) level = 0;
        statLevels[statType] = level;
        return level;
    }

    public void UpgradeLevel(UpgradableStatType statType)
    {
        if (!statLevels.ContainsKey(statType)) statLevels[statType] = 0;
        statLevels[statType]++;
    }
}
