using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public int credits;
    public List<WeaponType> ownedWeapons = new List<WeaponType>();
    public Dictionary<WeaponType, WeaponUpgradeState> weaponUpgrades = new Dictionary<WeaponType, WeaponUpgradeState>();
    public Dictionary<WeaponType, float> bulletsCount = new Dictionary<WeaponType, float>();
    public Dictionary<PlayerSkillsStats, float> playerStats = new Dictionary<PlayerSkillsStats, float>();

    public void printData()
    {
        Debug.Log($"Credits: {credits}");
        Debug.Log("Owned Weapons:");
        foreach (var weapon in ownedWeapons)
        {
            Debug.Log($"- {weapon}");
        }
        Debug.Log("Weapon Upgrades:");
        foreach (var kv in weaponUpgrades)
        {
            Debug.Log($"- {kv.Key}: {kv.Value}");
        }
        Debug.Log("Bullets Count:");
        foreach (var kv in bulletsCount)
        {
            Debug.Log($"- {kv.Key}: {kv.Value}");
        }
        Debug.Log("Player Stats:");
        foreach (var kv in playerStats)
        {
            Debug.Log($"- {kv.Key}: {kv.Value}");
        }
    }
}