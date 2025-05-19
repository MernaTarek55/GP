using UnityEngine;

public enum WeaponType
{
    None,
    Gun,
    Auto,
    GrenadeLauncher,
    Shotgun,
    Melee
}

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected float reloadTime;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected float damage;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float bulletForce;
    [SerializeField] protected float weaponDamage;

    public abstract void Shoot();
    public abstract void Reload();
}
