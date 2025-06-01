using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    protected int currentAmmo;
    [SerializeField] protected WeaponType WeaponType;
    protected WeaponData weaponData;

    protected virtual void Awake()
    {
        weaponData = WeaponDatabase.GetWeaponData(WeaponType);
        currentAmmo = weaponData.maxAmmo;
    }

    public abstract void Shoot(Vector3 target);
    public abstract void Reload();
}
