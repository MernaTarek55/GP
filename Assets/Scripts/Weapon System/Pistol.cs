using UnityEngine;

public class Pistol : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioSource audioSource;

    private float reloadTimer;
    private float fireCooldown;
    private bool isReloading;


    private void Awake()
    {
        weaponType = WeaponType.Gun;
        reloadTime = 2f;
        maxAmmo = 7;
        currentAmmo = maxAmmo;
        //damage = 30;
        damage = weaponDamage;
        fireRate = 0.5f;
        bulletForce = 20;
    }

    private void Update()
    {
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                currentAmmo = maxAmmo;
                isReloading = false;

                // Play reload sound
                if (audioSource && reloadSound)
                    audioSource.PlayOneShot(reloadSound);
            }
        }
    }

    public override void Shoot()
    {
        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
            return;

        fireCooldown = fireRate;

        currentAmmo--;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(bullet.transform.forward * bulletForce, ForceMode.Impulse);

        // Set bullet damage if it has a damage script
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(weaponDamage);
        }

        // Play muzzle flash
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Play shoot sound
        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);
    }

    public override void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            isReloading = true;
            reloadTimer = reloadTime;

            if (audioSource && reloadSound)
                audioSource.PlayOneShot(reloadSound);
        }
    }
}
