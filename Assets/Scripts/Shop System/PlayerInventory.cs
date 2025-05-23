using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventory
{
    HashSet<WeaponType> ownedWeapons = new();
    Dictionary<WeaponType, WeaponUpgradeState> weaponUpgrades = new();

    private int _credits;
    public event Action<int> OnCreditsChanged;

    public int Credits
    {
        get => _credits;
        set
        {
            if (_credits != value)
            {
                _credits = value;
                OnCreditsChanged?.Invoke(_credits);
            }
        }
    }

    
    public void InitializeWeaponUpgrades(List<WeaponData> weaponDataList)
    {
        Debug.Log("Initializing weapon upgrades...");
        foreach (WeaponData weaponData in weaponDataList)
        {
            Debug.Log($"Initializing weapon: {weaponData.weaponType}");
            var upgradeState = new WeaponUpgradeState();

            foreach (var stat in weaponData.upgradableStats)
            {
                Debug.Log($"Setting level for stat: {stat.statType}");
                upgradeState.SetLevel(stat.statType, 0);
            }

            weaponUpgrades[weaponData.weaponType] = upgradeState;
        }
    }

    //for testing
    public void PrintWeaponUpgrades()
    {
        foreach (var weapon in weaponUpgrades)
        {

            Debug.Log($"Weapon: {weapon.Key}, Upgrade State: {weapon.Value}");
            weapon.Value.PrintLevels();
        }
    }

    public WeaponUpgradeState GetUpgradeState(WeaponType type)
    {
        Debug.Log($"Getting upgrade state for weapon: {type}");
        if (weaponUpgrades.ContainsKey(type))
            return weaponUpgrades[type];

        return null;
    }
    public void SetUpgradeState(WeaponType type, WeaponUpgradeState state)
    {
        if (weaponUpgrades.ContainsKey(type))
            weaponUpgrades[type] = state;
        else
            weaponUpgrades.Add(type, state);
    }
    public void AddWeapon(WeaponType type)
    {
        if (!HasWeapon(type))
        {
            ownedWeapons.Add(type);
        }
    }
    public bool HasWeapon(WeaponType type) => ownedWeapons.Contains(type);
}
