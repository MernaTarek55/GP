using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class WeaponDatabase
{
    private static Dictionary<WeaponType, WeaponData> weaponDataMap;

    private static void LoadAll()
    {
        //this need to be redone
        if (weaponDataMap != null)
        {
            return;
        }

        WeaponData[] all = Resources.LoadAll<WeaponData>("WeaponData");
        weaponDataMap = all.ToDictionary(w => w.weaponType, w => w);
    }

    public static WeaponData GetWeaponData(WeaponType type)
    {
        if (weaponDataMap == null)
        {
            LoadAll();
        }
        //foreach (WeaponType key in weaponDataMap.Keys)
        //{
        //    Debug.Log("Dectanory Key " + key);
        //}
        return weaponDataMap[type];
    }

    public static IEnumerable<WeaponData> GetAllWeaponData()
    {
        if (weaponDataMap == null)
        {
            LoadAll();
        }
        return weaponDataMap.Values;
    }
}
