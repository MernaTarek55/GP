using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
public class AutoGun : Weapon
{
    Player player;

    [SerializeField] private DeadeyeSkill deadEye;

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

    private Vector3 shootDirection;

    [SerializeField] private Transform playerBody;

    [Header("UI")]
    [SerializeField] private List<GraphicRaycaster> uiRaycasters = new();
    [SerializeField] private EventSystem eventSystem;
    private readonly Dictionary<int, bool> touchStartedOverUI = new();

    private float totalAmmo; // Ammo in inventory
    private bool hasInfiniteAmmo = true;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();

        if (weaponData == null)
        {
            Debug.LogError("WeaponData not assigned in Inspector.");
            return;
        }

        // Initialize ammo from inventory
        currentAmmo = weaponData.maxAmmo;
        totalAmmo = PlayerInventoryHolder.instance.Inventory.GetAmmo(weaponData.weaponType);

        Debug.Log($"Weapon Type: {WeaponType}, Loaded: {currentAmmo}/{weaponData.maxAmmo}, Total: {totalAmmo}");
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

        // Check for touch input on mobile (hold)
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

    }

    private void ShootAtTouch(Vector2 screenPosition)
    {
        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.origin + (ray.direction * 100f);
        Vector3 lookDirection = targetPoint - playerBody.position;
        lookDirection.y = 0f; // Keep only horizontal rotation
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        playerBody.rotation = targetRotation;
        Shoot(targetPoint);
    }
    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        PointerEventData eventData = new(eventSystem)
        {
            position = screenPosition
        };

        foreach (var raycaster in uiRaycasters)
        {
            List<RaycastResult> results = new();
            raycaster.Raycast(eventData, results);
            if (results.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public override void Shoot(Vector3 targetPoint)
    {
        if (ikHandler != null)
        {
            ikHandler.TriggerShootIK();
            _ = StartCoroutine(WaitAndShootWhenIKReady(targetPoint));
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

    private IEnumerator WaitAndShootWhenIKReady(Vector3 targetPoint)
    {
        // Wait until IK weight is close to 1
        while (ikHandler.rig.weight < 0.8f)
        {
            yield return null;
        }

        shootDirection = (targetPoint - firePoint.position).normalized;
        firePoint.rotation = Quaternion.LookRotation(shootDirection);

        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
        {
            yield break;
        }

        fireCooldown = weaponData.fireRate;
        currentAmmo--;

        GameObject Laser = PoolManager.Instance.GetPrefabByTag(PoolType.Laser);
        Laser.GetComponent<Laser>().InitializeLaser(firePoint.transform.position, firePoint.transform.rotation, true);
        //Laser.transform.position = firePoint.transform.position;
        //Laser.transform.rotation = firePoint.transform.rotation;
        //Laser.SetActive(true);

        Rigidbody rb = Laser.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(Laser.transform.forward * weaponData.bulletForce, ForceMode.Impulse);
        //To Do
        //Laser bulletScript = Laser.GetComponent<Laser>();
        //if (bulletScript != null)
        //{
        //    // Get damage from weapon data and upgrades
        //    float baseDamage = weaponData.damage;
        //    var upgradeState = PlayerInventoryHolder.instance.Inventory.GetUpgradeState(weaponData.weaponType);
        //    float damageMultiplier = 1f + (upgradeState?.GetLevel(UpgradableStatType.Damage) ?? 0) * 0.1f; // 10% per level
        //    bulletScript.SetDamage(baseDamage * damageMultiplier);
        //}

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
        if (isReloading || currentAmmo == weaponData.maxAmmo)
            return;

        // Check if we have ammo to reload
        if (totalAmmo <= 0 && !hasInfiniteAmmo)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        isReloading = true;
        reloadTimer = weaponData.reloadTime;

        // Calculate how much ammo to add
        int ammoNeeded = weaponData.maxAmmo - currentAmmo;
        int ammoToAdd = Mathf.Min(ammoNeeded, (int)totalAmmo);

        // For infinite ammo mode (debug/cheat)
        if (hasInfiniteAmmo)
        {
            ammoToAdd = ammoNeeded;
        }
        else
        {
            totalAmmo -= ammoToAdd;
            PlayerInventoryHolder.instance.Inventory.SetAmmo(weaponData.weaponType, totalAmmo);
        }

        currentAmmo += ammoToAdd;

        if (audioSource && reloadSound)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        Debug.Log($"Reloaded: +{ammoToAdd}, Now: {currentAmmo}/{weaponData.maxAmmo}, Total: {totalAmmo}");
    }
    public (int current, int total) GetAmmoInfo()
    {
        return (currentAmmo, (int)totalAmmo);
    }
}
