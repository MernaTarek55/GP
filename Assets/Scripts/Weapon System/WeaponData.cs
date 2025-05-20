using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;
    public float reloadTime;
    public int maxAmmo;
    public float damage;
    public float fireRate;
    public float bulletForce;
}
