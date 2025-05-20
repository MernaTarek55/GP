using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class WeaponDatabase
{
    private static Dictionary<WeaponType, WeaponData> weaponDataMap;

    public static void LoadAll()
    {
        var all = Resources.LoadAll<WeaponData>("WeaponData");
        weaponDataMap = all.ToDictionary(w => w.weaponType, w => w);
    }

    public static WeaponData GetWeaponData(WeaponType type)
    {
        return weaponDataMap[type];
    }

    public static IEnumerable<WeaponData> GetAllWeaponData()
    {
        return weaponDataMap.Values;
    }
}
