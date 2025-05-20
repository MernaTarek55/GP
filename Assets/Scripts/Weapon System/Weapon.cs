using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponData weaponData;
    protected int currentAmmo;
    private void Start()
    {
        currentAmmo = weaponData.maxAmmo;
    }

    public abstract void Shoot();
    public abstract void Reload();
}
