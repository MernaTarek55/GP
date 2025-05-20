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
[CreateAssetMenu(menuName = "Weapons/WeaponData")]
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
    public List<UpgradableStat> statUpgrades;
    public UpgradableStat GetStatUpgrade(UpgradableStatType type)
    {
        return statUpgrades.Find(stat => stat.statType == type);
    }

}