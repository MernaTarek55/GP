using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventory
{
    public HashSet<WeaponType> ownedWeapons = new();
    public Dictionary<WeaponType, WeaponUpgradeState> weaponUpgrades = new();

    public int credits;
    public void InitializeWeaponUpgrades(List<WeaponData> weaponDataList)
    {
        foreach (WeaponData weaponData in weaponDataList)
        {
            var upgradeState = new WeaponUpgradeState();

            foreach (var stat in weaponData.upgradableStats)
            {
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
            weaponUpgrades[type] = new WeaponUpgradeState();
        }
    }
    public bool HasWeapon(WeaponType type) => ownedWeapons.Contains(type);
}
