﻿////using UnityEngine;
////using UnityEngine.EventSystems;
////using UnityEngine.UI;
////using System.Collections.Generic;
////using System.Collections;
////public class GrenadeLauncher : Weapon
////{
////    [SerializeField] private GameObject grenadePrefab;
////    [SerializeField] private Transform firePoint;

////    [Header("Effects")]
////    [SerializeField] private ParticleSystem muzzleFlash;
////    [SerializeField] private AudioClip shootSound;
////    [SerializeField] private AudioClip reloadSound;
////    [SerializeField] private AudioSource audioSource;

////    [SerializeField] private IKHandler ikHandler;
////    [SerializeField] private Transform playerBody;

////    [Header("UI")]
////    [SerializeField] private GraphicRaycaster uiRaycaster;
////    [SerializeField] private EventSystem eventSystem;

////    [Header("Launcher")]
////    [SerializeField] private float upwardMultiplier = 0.5f;
////    [SerializeField] private float forwardForce = 20f;

////    private Dictionary<int, bool> touchStartedOverUI = new Dictionary<int, bool>();

////    private float fireCooldown;
////    private float reloadTimer;
////    private bool isReloading;
////    private Vector3 targetPoint;
////    private Vector3 shootDirection;

////    private void Awake()
////    {
////        base.Awake();
////        if (weaponData == null)
////        {
////            Debug.LogError("WeaponData not assigned in Inspector.");
////            return;
////        }
////        currentAmmo = weaponData.maxAmmo;
////        Debug.Log($"Weapon Type: {WeaponType}");
////    }

////    private void Update()
////    {
////        if (fireCooldown > 0)
////            fireCooldown -= Time.deltaTime;

////        if (!isReloading && currentAmmo == 0)
////            Reload();

////        if (isReloading)
////        {
////            reloadTimer -= Time.deltaTime;
////            if (reloadTimer <= 0f)
////            {
////                currentAmmo = weaponData.maxAmmo;
////                isReloading = false;
////                if (audioSource && reloadSound)
////                    audioSource.PlayOneShot(reloadSound);
////            }
////        }

////        // Check for touch input on mobile
////        for (int i = 0; i < Input.touchCount; i++)
////        {
////            Touch touch = Input.GetTouch(i);
////            int fingerId = touch.fingerId;

////            if (touch.phase == TouchPhase.Began)
////            {
////                // On first touch, record whether it started over UI
////                bool isOverUI = IsTouchOverUI(touch.position);
////                touchStartedOverUI[fingerId] = isOverUI;
////            }
////            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
////            {
////                // Only allow shooting if this finger started off-UI
////                if (touchStartedOverUI.TryGetValue(fingerId, out bool startedOverUI) && !startedOverUI)
////                {
////                    ShootAtTouch(touch.position);
////                }
////            }
////            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
////            {
////                // Clean up dictionary when touch ends
////                touchStartedOverUI.Remove(fingerId);
////            }
////        }
////    }

////    private void ShootAtTouch(Vector2 screenPosition)
////    {
////        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
////        {
////            return;
////        }

////        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
////        RaycastHit hit;

////        if (Physics.Raycast(ray, out hit))
////        {
////            targetPoint = hit.point;
////        }
////        else
////        {
////            targetPoint = ray.origin + ray.direction * 100f;
////        }
////        Vector3 lookDirection = (targetPoint - playerBody.position);
////        lookDirection.y = 0f; // Keep only horizontal rotation
////        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
////        playerBody.rotation = targetRotation;
////        Shoot();
////    }

////    public override void Reload()
////    {
////        if (!isReloading && currentAmmo < weaponData.maxAmmo)
////        {
////            isReloading = true;
////            reloadTimer = weaponData.reloadTime;

////            if (audioSource && reloadSound)
////                audioSource.PlayOneShot(reloadSound);
////        }
////    }

////    public override void Shoot()
////    {
////        if (ikHandler != null)
////        {
////            ikHandler.TriggerShootIK();
////            StartCoroutine(WaitAndShootWhenIKReady());
////        }
////    }

////    private bool IsTouchOverUI(Vector2 screenPosition)
////    {
////        PointerEventData eventData = new PointerEventData(eventSystem);
////        eventData.position = screenPosition;

////        List<RaycastResult> results = new List<RaycastResult>();
////        uiRaycaster.Raycast(eventData, results);

////        return results.Count > 0;
////    }

////    private IEnumerator WaitAndShootWhenIKReady()
////    {
////        while (ikHandler.rig.weight < 0.8f)
////            yield return null;

////        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
////            yield break;

////        fireCooldown = weaponData.fireRate;
////        currentAmmo--;

////        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);
////        Rigidbody rb = grenade.GetComponent<Rigidbody>();
////        grenade.transform.parent = null;
////        rb.isKinematic = false;

////        // Basic lob force
////        Vector3 shootDir = (targetPoint - firePoint.position).normalized;

////        // Control curve height with upward force multiplier
////        float forwardForce = weaponData.bulletForce;
////        float upwardMultiplier = 0.5f; // Change this to tweak the arc
////        Vector3 force = shootDir * forwardForce + Vector3.up * forwardForce * upwardMultiplier;

////        rb.AddForce(force, ForceMode.Impulse);

////        if (muzzleFlash) muzzleFlash.Play();
////        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

////        //Bullet bulletScript = grenade.GetComponent<Bullet>();
////        //if (bulletScript != null)
////        //    bulletScript.SetDamage(weaponData.damage);
////    }

////}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Animations.Rigging;

public class GrenadeLauncher : Weapon
{
    Player player;

    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioSource audioSource;


    [SerializeField] private Transform playerBody;

    [Header("UI")]
    [SerializeField] private List<GraphicRaycaster> uiRaycasters = new();
    [SerializeField] private EventSystem eventSystem;

    [Header("Launcher")]
    [SerializeField] private float upwardMultiplier = 0.5f;
    [SerializeField] private float forwardForce = 20f;

    [Header("Trajectory")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPoints = 30;
    [SerializeField] private float timeBetweenPoints = 0.05f;
    [SerializeField] private LayerMask collisionMask;

    private Dictionary<int, bool> touchStartedOverUI = new Dictionary<int, bool>();
    private float fireCooldown;
    private float reloadTimer;
    private bool isReloading;
    private Vector3 targetPoint;
    private Vector3 shootDirection;
    private PlayerInventoryHolder playerInventoryHolder;
    WeaponUpgradeState upgradeState ;
    [Header("Rig")]
    [SerializeField] private Transform Spher;
    [SerializeField] private Rig rig;
    [SerializeField] private float littleTimer = 0.0f;
    [SerializeField] private float littleTimerMax = 1.0f;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
        playerInventoryHolder = SaveManager.Singleton.playerInventoryHolder;

        
        if (weaponData == null)
        {
            Debug.LogError("WeaponData not assigned in Inspector.");
            return;
        }
        littleTimer = littleTimerMax;
        currentAmmo = weaponData.maxAmmo;
        Debug.Log($"Weapon Type: {WeaponType}");
    }

    private void Start()
    {
        upgradeState = playerInventoryHolder.Inventory.GetUpgradeState(WeaponType);
        if (upgradeState == null) Debug.LogWarning("Upgrade state is null for weapon: " + WeaponType);
    }

    private void Update()
    {
        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;

        if (!isReloading && currentAmmo == 0)
            Reload();

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

        // Touch Input
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
        if (littleTimer > 0)
        {
            littleTimer -= Time.deltaTime;
        }
        else
        {
            rig.weight = 0;
        }
    }

    private void ShowTrajectory(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }

        Vector3 shootDir = (targetPoint - firePoint.position).normalized;
        Vector3 force = shootDir * forwardForce + Vector3.up * forwardForce * upwardMultiplier;

        DrawTrajectory(firePoint.position, force);
    }

    private void DrawTrajectory(Vector3 startPos, Vector3 initialVelocity)
    {
        Vector3[] points = new Vector3[trajectoryPoints];

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeBetweenPoints;
            Vector3 point = startPos + initialVelocity * t + 0.5f * Physics.gravity * t * t;

            points[i] = point;

            if (i > 0)
            {
                Vector3 segment = points[i] - points[i - 1];
                if (Physics.Raycast(points[i - 1], segment.normalized, out RaycastHit hit, segment.magnitude, collisionMask))
                {
                    points[i] = hit.point;
                    System.Array.Resize(ref points, i + 1);
                    break;
                }
            }
        }

        trajectoryLine.positionCount = points.Length;
        trajectoryLine.SetPositions(points);
        trajectoryLine.enabled = true;
    }

    private void ShootAtTouch(Vector2 screenPosition)
    {
        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
      

       
        targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.origin + (ray.direction * 100f);
        Spher.position = targetPoint;
        rig.weight = 1;
        littleTimer = littleTimerMax;
        Vector3 lookDirection = targetPoint - playerBody.position;
        lookDirection.y = 0f; // Keep only horizontal rotation
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        playerBody.rotation = targetRotation;
        Shoot(targetPoint);
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
    Vector3 targetForAnimations;
    public override void Shoot(Vector3 targetPoint)
    {
        targetForAnimations = targetPoint;
        animator.SetBool("shoot", true);
        
    }
    public override void ShootFromAnimation()
    {
        base.ShootFromAnimation();
        //if (currentAmmo <= 0 || isReloading) return;

        StartCoroutine(WaitAndShootWhenIKReady());
    }
    private IEnumerator WaitAndShootWhenIKReady()
    {
       

        if (isReloading || currentAmmo <= 0 || fireCooldown > 0f)
            yield break;

        //fireCooldown = weaponData.fireRate;
        fireCooldown = upgradeState.GetLevel(UpgradableStatType.FireRate);
        Debug.LogWarning(fireCooldown + " fire rate");
        currentAmmo--;

        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        // reset liner and angular velocity to make the bullet hit right
        rb.linearVelocity = Vector3.zero; // reset!
        rb.angularVelocity = Vector3.zero;

        grenade.transform.parent = null;
        rb.isKinematic = false;

        Vector3 shootDir = (targetPoint - firePoint.position).normalized;
        //Vector3 force = shootDir * forwardForce + Vector3.up * forwardForce * upwardMultiplier;
        float upwardForce = (targetPoint - firePoint.position).magnitude * upwardMultiplier;
        Vector3 force = shootDir * forwardForce + Vector3.up * upwardForce;
        rb.AddForce(force, ForceMode.Impulse);

        if (muzzleFlash) muzzleFlash.Play();
        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

        trajectoryLine.enabled = false;

        ShellBullet bulletScript = grenade.GetComponent<ShellBullet>();
        if (bulletScript != null)
        {
            Debug.LogWarning(weaponData.damage * upgradeState.GetLevel(UpgradableStatType.Damage));
            bulletScript.SetDamage(weaponData.damage*upgradeState.GetLevel(UpgradableStatType.Damage));
        }
        animator.SetBool("shoot", false);
    }

    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = screenPosition;

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
}

