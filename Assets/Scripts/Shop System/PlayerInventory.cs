using System.Collections.Generic;

public class PlayerInventory
{
    public HashSet<WeaponType> ownedWeapons = new();
    public Dictionary<WeaponType, WeaponUpgradeState> weaponUpgrades = new();

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
