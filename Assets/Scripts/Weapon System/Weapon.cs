using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    WeaponData weaponData;
    private int currentAmmo;
    private void Start()
    {
        currentAmmo = weaponData.maxAmmo;
    }

    public abstract void Shoot();
    public abstract void Reload();
}
