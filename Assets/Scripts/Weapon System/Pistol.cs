using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pistol : Weapon
{
    Player player;

    [SerializeField] private DeadeyeSkill deadEye;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    //better in parent??
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

    //Dictionary
    private readonly Dictionary<int, bool> touchStartedOverUI = new();

    private void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();

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
        {
            fireCooldown -= Time.deltaTime;
        }

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
                {
                    audioSource.PlayOneShot(reloadSound);
                }
            }
        }

        // Check for touch input on mobile
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            int fingerId = touch.fingerId;

            if (touch.phase == TouchPhase.Began)
            {
                bool isOverUI = IsTouchOverUI(touch.position);
                touchStartedOverUI[fingerId] = isOverUI;
            }
            else if (touch.phase is TouchPhase.Stationary or TouchPhase.Moved)
            {
                if (touchStartedOverUI.TryGetValue(fingerId, out bool startedOverUI) && !startedOverUI)
                {
                    player?.SetShooting(true); // ✅ START shooting flag
                    ShootAtTouch(touch.position);
                }
            }
            else if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled)
            {
                touchStartedOverUI.Remove(fingerId);
                player?.SetShooting(false); // ✅ END shooting flag
            }
        }

        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && deadEye.canShoot == true)
        //{
        //    Touch touch = Input.GetTouch(0);
        //    int fingerId = touch.fingerId;

        //    if (touch.phase == TouchPhase.Began)
        //    {
        //        // On first touch, record whether it started over UI
        //        bool isOverUI = IsTouchOverUI(touch.position);
        //        touchStartedOverUI[fingerId] = isOverUI;
        //    }
        //    else if (touch.phase is TouchPhase.Stationary or TouchPhase.Moved)
        //    {
        //        // Only allow shooting if this finger started off-UI
        //        if (touchStartedOverUI.TryGetValue(fingerId, out bool startedOverUI) && !startedOverUI)
        //        {
        //            Debug.Log("deadEye.canShoot///////////////////////////");
        //            Debug.Log(deadEye.canShoot);
        //            ShootAtTouch(touch.position);
        //        }
        //    }
        //    else if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled)
        //    {
        //        // Clean up dictionary when touch ends
        //        _ = touchStartedOverUI.Remove(fingerId);
        //    }
        //}
    }

    private void ShootAtTouch(Vector2 screenPosition)
    {
        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.origin + (ray.direction * 100f);
        Vector3 lookDirection = targetPoint - playerBody.position;
        lookDirection.y = 0f; // Keep only horizontal rotation
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        playerBody.rotation = targetRotation;
        Shoot();
    }
    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        PointerEventData eventData = new(eventSystem)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new();
        uiRaycaster.Raycast(eventData, results);

        return results.Count > 0;
    }
    public override void Shoot()
    {
        if (ikHandler != null)
        {
            ikHandler.TriggerShootIK();
            _ = StartCoroutine(WaitAndShootWhenIKReady());
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
        {
            yield break;
        }

        fireCooldown = weaponData.fireRate;
        currentAmmo--;

        GameObject bullet = PoolManager.Instance.GetPrefabByTag(PoolType.Bullet);
        bullet.transform.position = firePoint.transform.position;
        bullet.transform.rotation = firePoint.transform.rotation;
        bullet.SetActive(true);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.AddForce(bullet.transform.forward * weaponData.bulletForce, ForceMode.Impulse);


        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            //to change to the data in inventory 
            bulletScript.SetDamage(5/*weaponData.damage*/);
        }
           // bulletScript.SetDamage(weaponData.damage);

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    public override void Reload()
    {
        if (!isReloading && currentAmmo < weaponData.maxAmmo)
        {
            isReloading = true;
            reloadTimer = weaponData.reloadTime;

            if (audioSource && reloadSound)
            {
                audioSource.PlayOneShot(reloadSound);
            }
        }
    }
}
