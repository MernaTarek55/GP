using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    Player player;
    protected int currentAmmo;
    [SerializeField] public WeaponType WeaponType;
    protected WeaponData weaponData;
    protected Animator animator;

    protected virtual void Awake()
    {
        weaponData = WeaponDatabase.GetWeaponData(WeaponType);
        currentAmmo = weaponData.maxAmmo;
        player = GetComponentInParent<Player>();
        animator = player.gameObject.GetComponent<Animator>();
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
