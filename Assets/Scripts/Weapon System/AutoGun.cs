using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
public class AutoGun : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private IKHandler ikHandler;

    private float reloadTimer;
    private float fireCooldown;
    private bool isReloading;

    private Vector3 targetPoint;
    private Vector3 shootDirection;

    [SerializeField] private Transform playerBody;

    [Header("UI")]
    [SerializeField] private GraphicRaycaster uiRaycaster;
    [SerializeField] private EventSystem eventSystem;
    private void Awake()
    {
        base.Awake();
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

        if (!isReloading && currentAmmo == 0)
        {
            Reload();
        }

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

        // Check for touch input on mobile (hold)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (IsTouchOverUI(touch.position))
            {
                Debug.Log("Touch is on UI — not shooting");
                return;
            }
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                Vector2 touchPos = touch.position;
                ShootAtTouch(touchPos);
            }
        }
    }

    private void ShootAtTouch(Vector2 screenPosition)
    {
        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }
        Vector3 lookDirection = (targetPoint - playerBody.position);
        lookDirection.y = 0f; // Keep only horizontal rotation
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        playerBody.rotation = targetRotation;
        Shoot();
    }
    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventData, results);

        return results.Count > 0;
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

        // moved these here - to get the final firePoint calculations after doing the ik
        shootDirection = (targetPoint - firePoint.position).normalized;
        firePoint.rotation = Quaternion.LookRotation(shootDirection);

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
