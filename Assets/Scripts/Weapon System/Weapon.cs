using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    protected int currentAmmo;
    [SerializeField] public WeaponType WeaponType;
    protected WeaponData weaponData;

    protected virtual void Awake()
    {
        weaponData = WeaponDatabase.GetWeaponData(WeaponType);
        currentAmmo = weaponData.maxAmmo;
    }

    public abstract void Shoot(Vector3 target);
    public abstract void Reload();


    public virtual void ShootFromAnimation()
    {
        Debug.Log($"{WeaponType} shoot triggered from animation.");
    }
    public float GetFireRate()
    {
        return weaponData.fireRate;
    }

    public virtual IEnumerator ShootForDeadEye(Vector3 target)
    {
        //Shoot(target);
        yield return null;
    }
}
