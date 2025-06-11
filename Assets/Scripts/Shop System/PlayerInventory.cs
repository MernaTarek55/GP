using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class PlayerInventory
{
    public InventorySaveData inventorySaveData = new InventorySaveData();

    public event Action<int> OnCreditsChanged;

    public int Credits
    {
        get => inventorySaveData.credits;
        set
        {
            if (inventorySaveData.credits != value)
            {
                inventorySaveData.credits = value;
                OnCreditsChanged?.Invoke(inventorySaveData.credits);
            }
        }
    }

    public float MaxHealth
    {
        get => getPlayerStat(PlayerSkillsStats.MaxHealth);
        set => SetPlayerStat(PlayerSkillsStats.MaxHealth, value);
    }
    public float CurrentHealth
    {
        get => inventorySaveData.currentHealth;
        set
        {
            inventorySaveData.currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            // You might want to add an OnHealthChanged event here
        }
    }
    public void InitializePlayerStats()
    {
        // Set default health if not already set
        if (!inventorySaveData.playerStats.ContainsKey(PlayerSkillsStats.MaxHealth))
        {
            SetPlayerStat(PlayerSkillsStats.MaxHealth, 100f); // Default max health
        }
        CurrentHealth = MaxHealth; // Start with full health
    }
    public void InitializeWeaponUpgrades(List<WeaponData> weaponDataList)
    {
        Debug.Log("Initializing weapon upgrades...");
        foreach (WeaponData weaponData in weaponDataList)
        {
            Debug.Log($"Initializing weapon: {weaponData.weaponType}");
            WeaponUpgradeState upgradeState = new WeaponUpgradeState();

            foreach (UpgradableStat stat in weaponData.upgradableStats)
            {
                Debug.Log($"Setting level for stat: {stat.statType}");
                upgradeState.SetLevel(stat.statType, 0);
            }
            if (inventorySaveData.weaponUpgrades.ContainsKey(weaponData.weaponType))
            {
                inventorySaveData.weaponUpgrades[weaponData.weaponType] = upgradeState;
            }
            else
            {
                inventorySaveData.weaponUpgrades.Add(weaponData.weaponType, upgradeState);
            }
        }
    }

    //for testing
    public void PrintWeaponUpgrades()
    {
        foreach (KeyValuePair<WeaponType, WeaponUpgradeState> weapon in inventorySaveData.weaponUpgrades)
        {

            Debug.Log($"Weapon: {weapon.Key}, Upgrade State: {weapon.Value}");
            weapon.Value.PrintLevels();
        }
    }

    public WeaponUpgradeState GetUpgradeState(WeaponType type)
    {
        Debug.Log($"Getting upgrade state for weapon: {type}");
        return inventorySaveData.weaponUpgrades.ContainsKey(type) ? inventorySaveData.weaponUpgrades[type] : null;
    }
    public void SetUpgradeState(WeaponType type, WeaponUpgradeState state)
    {
        if (inventorySaveData.weaponUpgrades.ContainsKey(type))
        {
            inventorySaveData.weaponUpgrades[type] = state;
        }
        else
        {
            inventorySaveData.weaponUpgrades.Add(type, state);
        }
    }

    public float getPlayerStat(PlayerSkillsStats stat)
    {
        if (inventorySaveData.playerStats.TryGetValue(stat, out float value))
        {
            Debug.Log($"Player stat {stat} value: {value}");
            return value;
        }
        Debug.LogWarning($"Player stat {stat} not found, returning 0.");
        return 0f;
    }
    public void SetPlayerStat(PlayerSkillsStats stat, float value)
    {
        if (inventorySaveData.playerStats.ContainsKey(stat))
        {
            inventorySaveData.playerStats[stat] = value;
        }
        else
        {
            inventorySaveData.playerStats.Add(stat, value);
        }
        Debug.Log($"Set player stat {stat} to {value}");
    }
    public void UpgradePlayerStat(PlayerSkillsStats stat)
    {
        if (inventorySaveData.playerStats.ContainsKey(stat))
        {
            inventorySaveData.playerStats[stat]++;
        }
    }
    public float GetAmmo(WeaponType weapon)
    {
        return inventorySaveData.bulletsCount.ContainsKey(weapon) ? inventorySaveData.bulletsCount[weapon] : 0f;
    }
    public void SetAmmo(WeaponType weapon, float value)
    {
        if (inventorySaveData.bulletsCount.ContainsKey(weapon))
        {
            inventorySaveData.bulletsCount[weapon] = value;
        }
    }


    public void AddWeapon(WeaponType type)
    {
        if (!HasWeapon(type))
        {
            inventorySaveData.ownedWeapons.Add(type);
        }
    }
    public bool HasWeapon(WeaponType type)
    {
        return inventorySaveData.ownedWeapons.Contains(type);
    }
}
