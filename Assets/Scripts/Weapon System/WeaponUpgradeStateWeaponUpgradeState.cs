using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponUpgradeState
{
    private readonly Dictionary<UpgradableStatType, int> statLevels = new();

    public int GetLevel(UpgradableStatType statType)
    {
        return statLevels.TryGetValue(statType, out int level) ? level : 0;
    }
    
    public int SetLevel(UpgradableStatType statType, int level)
    {
        if (level < 0)
        {
            level = 0;
        }

        statLevels[statType] = level;
        return level;
    }
    //for testing
    public void PrintLevels()
    {
        foreach (KeyValuePair<UpgradableStatType, int> stat in statLevels)
        {
            Debug.Log($"Stat: {stat.Key}, Level: {stat.Value}");
        }
    }

    public void UpgradeLevel(UpgradableStatType statType)
    {
        if (!statLevels.ContainsKey(statType))
        {
            statLevels[statType] = 0;
        }

        statLevels[statType]++;
    }
}
