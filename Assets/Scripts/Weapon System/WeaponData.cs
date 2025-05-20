using UnityEngine;

public enum WeaponType
{
    None,
    Gun,
    Auto,
    GrenadeLauncher,
    Melee
}
[CreateAssetMenu(menuName = "Weapons/BasicWeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;
    public float reloadTime;
    public int maxAmmo;

    //What is the difference??
    public float damage;
    public float weaponDamage;
    //

    public float fireRate;
    public float bulletForce;
}
[CreateAssetMenu(menuName = "Weapons/GrenadeLauncherData")]
public class GrenadeLauncherData : WeaponData
{
    public float areaOfEffectRadius;
}