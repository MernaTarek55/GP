using System.Collections;
using Unity.VisualScripting;
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

    [SerializeField] private IKHandler ikHandler;
    private void Awake()
    {
        if (weaponData == null)
        {
            Debug.LogError("WeaponData not assigned in Inspector.");
            return;
        }
        currentAmmo = weaponData.maxAmmo;
        Debug.Log($"Weapon Type: {WeaponType}");
    }

    private void Update()
    {
        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                currentAmmo = weaponData.maxAmmo;
                isReloading = false;

                if (audioSource && reloadSound)
                    audioSource.PlayOneShot(reloadSound);
            }
        }

        // Check for touch input on mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPos = Input.GetTouch(0).position;
            ShootAtTouch(touchPos);
        }
    }
    private void ShootAtTouch(Vector2 screenPosition)
    {
        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }

        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        firePoint.rotation = Quaternion.LookRotation(shootDirection);

        Shoot();
    }
    public override void Shoot()
    {
        if (ikHandler != null)
        {
            ikHandler.TriggerShootIK();
            StartCoroutine(WaitAndShootWhenIKReady());
        }
        //if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
        //    return;

        //fireCooldown = weaponData.fireRate;
        //currentAmmo--;

        //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        //Rigidbody rb = bullet.GetComponent<Rigidbody>();
        //rb.AddForce(bullet.transform.forward * weaponData.bulletForce, ForceMode.Impulse);

        //Bullet bulletScript = bullet.GetComponent<Bullet>();
        //if (bulletScript != null)
        //    bulletScript.SetDamage(weaponData.damage);

        //if (muzzleFlash != null)
        //    muzzleFlash.Play();

        //if (audioSource && shootSound)
        //    audioSource.PlayOneShot(shootSound);
    }
    private IEnumerator WaitAndShootWhenIKReady()
    {
        // Wait until IK weight is close to 1
        while (ikHandler.rig.weight < 0.8f)
        {
            yield return null; // wait for next frame
        }

        // Only shoot if allowed
        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
            yield break;

        fireCooldown = weaponData.fireRate;
        currentAmmo--;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(bullet.transform.forward * weaponData.bulletForce, ForceMode.Impulse);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
            bulletScript.SetDamage(weaponData.damage);

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);
    }

    public override void Reload()
    {
        if (!isReloading && currentAmmo < weaponData.maxAmmo)
        {
            isReloading = true;
            reloadTimer = weaponData.reloadTime;

            if (audioSource && reloadSound)
                audioSource.PlayOneShot(reloadSound);
        }
    }
}
