using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class InventorySaveManager
{

    private readonly string savePath = Path.Combine(Application.persistentDataPath, "inventory.json");

    private readonly List<WeaponType> ownedWeapons;
    private readonly Dictionary<WeaponType, WeaponUpgradeState> weaponUpgrades;
    private readonly Dictionary<WeaponType, float> bulletsCount;
    private readonly Dictionary<PlayerSkillsStats, float> playerStats;

    private int credits;
    private int lastPlayedLevelIndex;

    public InventorySaveManager(
        List<WeaponType> ownedWeapons,
        Dictionary<WeaponType, WeaponUpgradeState> weaponUpgrades,
        Dictionary<WeaponType, float> bulletsCount,
        Dictionary<PlayerSkillsStats, float> playerStats,
        int credits,
        int lastPlayedLevelIndex
    )
    {
        this.ownedWeapons = ownedWeapons;
        this.weaponUpgrades = weaponUpgrades;
        this.bulletsCount = bulletsCount;
        this.playerStats = playerStats;
        this.credits = credits;
        this.lastPlayedLevelIndex = lastPlayedLevelIndex;
    }

    public void Save()
    {
        InventorySaveData saveData = new InventorySaveData
        {
            credits = credits,
            lastPlayedLevelIndex = lastPlayedLevelIndex,
            ownedWeapons = ownedWeapons.ToList(),
            weaponUpgrades = new Dictionary<WeaponType, WeaponUpgradeState>(weaponUpgrades),
            bulletsCount = new Dictionary<WeaponType, float>(bulletsCount),
            playerStats = new Dictionary<PlayerSkillsStats, float>(playerStats)
        };

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(savePath, json);
        Debug.Log("Inventory saved to: " + savePath);
    }

    public InventorySaveData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Inventory Save file not found, creating new save.");
            return null;
        }
        Debug.Log("Loading Inventory");
        string json = File.ReadAllText(savePath);
        InventorySaveData data = JsonConvert.DeserializeObject<InventorySaveData>(json);
        return data;
    }
}
