using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    None,
    Gun,
    Auto,
    GrenadeLauncher,
    Melee
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;
    public float reloadTime;
    public int maxAmmo;

    public float bulletForce;

    //What is the difference??
    //public float damage;
    //public float weaponDamage;

    //Upgrdables
    public List<UpgradableStat> upgradableStats;
    public UpgradableStat GetUpgradableStat(UpgradableStatType type)
    {
        return upgradableStats.Find(stat => stat.statType == type);
    }


    public float damage;
    public float fireRate;
    public float bulletForce;
}
