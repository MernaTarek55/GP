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
    [SerializeField] protected WeaponData weaponData;

    protected int currentAmmo;
    public WeaponType WeaponType => weaponData.weaponType;

    protected virtual void Awake()
    {
        currentAmmo = weaponData.maxAmmo;
    }

    public abstract void Shoot();
    public abstract void Reload();
}
