using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Pistol : Weapon
{
    Player player;

    [SerializeField] private DeadeyeSkill deadEye;
    [SerializeField] private bool deadEyeBool = false;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    //better in parent??
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioSource audioSource;


    private float reloadTimer;
    private float fireCooldown;
    private bool isReloading;

    //private Vector3 targetPoint;
    private Vector3 shootDirection;

    [SerializeField] private Transform playerBody;
    [Header("UI")]
    [SerializeField] private List<GraphicRaycaster> uiRaycasters = new();
    [SerializeField] private EventSystem eventSystem;


    //Dictionary
    private readonly Dictionary<int, bool> touchStartedOverUI = new();

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();

        if (weaponData == null)
        {
            Debug.LogError("WeaponData not assigned in Inspector.");
            return;
        }
        currentAmmo = weaponData.maxAmmo;
        //Debug.Log($"Weapon Type: {WeaponType}");
    }

    private void Update()
    {
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.unscaledDeltaTime;
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
                if (touchStartedOverUI.TryGetValue(fingerId, out bool startedOverUI) && !startedOverUI && deadEye.canShoot == true)
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
    public override void Shoot(Vector3 targetPoint) // add parameter the target you want to shoot
    {
        //if (ikHandler != null)
        //{
        //    ikHandler.TriggerShootIK();
        //}
        player.gameObject.GetComponent<Animator>().SetTrigger("Shoot");
        StartCoroutine(WaitAndShootWhenIKReady(targetPoint));
        deadEyeBool = false;
        //player.gameObject.GetComponent<Animator>().ResetTrigger("Shoot");
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
        //while (ikHandler.rig.weight < 0.8f)
        //{
        //    yield return null; // wait for next frame
        //}

        // moved these here - to get the final firePoint calculations after doing the ik
        shootDirection = (targetPoint - firePoint.position).normalized;
        firePoint.rotation = Quaternion.LookRotation(shootDirection);

        // Only shoot if allowed
        if (isReloading && fireCooldown > 0f)
        {
            yield break;
        }
        if (!deadEyeBool)
        {
            fireCooldown = weaponData.fireRate;

        }
        currentAmmo--;
        if(WeaponType == WeaponType.Auto)
        {
            GameObject Laser = PoolManager.Instance.GetPrefabByTag(PoolType.Laser);
            Laser.GetComponent<Laser>().InitializeLaser(firePoint.transform.position, firePoint.transform.rotation, true);
            Laser.GetComponent<Laser>().SetLaserDamage(weaponData.damage);
            AudioManager.Instance.PlaySound(SoundType.Laser);
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
        }
        else
        {
            GameObject bullet = /*Instantiate(bulletPrefab , firePoint.position , Quaternion.identity);*/PoolManager.Instance.GetPrefabByTag(PoolType.Bullet);
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.LookRotation(shootDirection); // make sure it's updated
            bullet.SetActive(true);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            // reset liner and angular velocity to make the bullet hit right
            rb.linearVelocity = Vector3.zero; // reset!
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(bullet.transform.forward * weaponData.bulletForce, ForceMode.Impulse);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                //to change to the data in inventory 
                bulletScript.SetDamage(5/*weaponData.damage*/);

            }
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


    public void ShootTargets(List<Transform> targets)
    {
        StartCoroutine(ShootTargetsSequentially(targets));
        //if (ikHandler != null)
        //{
        //    ikHandler.TriggerShootIK();
        //    //_ = StartCoroutine(WaitAndShootWhenIKReady(targetPoint));
        //}
    }


    private IEnumerator ShootTargetsSequentially(List<Transform> targets)
    {
        //while (ikHandler.rig.weight < 0.8f)
        //{
        //    yield return null; // wait for next frame
        //}

        deadEye.canShoot = false;
        currentAmmo = weaponData.maxAmmo;


        //Invoke("etfo", targets, weaponData.fireRate);


        deadEye.canShoot = true;
        currentAmmo = weaponData.maxAmmo;

        yield return null;
    }

    //private IEnumerator nady3laetfo(List<Transform> targets)
    //{
    //    yield return new WaitForSeconds(weaponData.fireRate);

    //    Debug.Log($"fire rate = {weaponData.fireRate}");

    //    if (targets.Count > 0)
    //    {
    //        etfo(targets);
    //    }
    //}

    //private void etfo(List<Transform> targets)
    //{
    //    shootDirection = (targets[targets.Count - 1].position - firePoint.position).normalized;
    //    firePoint.rotation = Quaternion.LookRotation(shootDirection);


    //    GameObject bullet = PoolManager.Instance.GetPrefabByTag(PoolType.Bullet);

    //    Debug.Log($"bullet is {(bullet == null ? "NULL" : "OK")}.");

    //    bullet.transform.position = firePoint.position;
    //    bullet.transform.rotation = Quaternion.LookRotation(shootDirection); // make sure it's updated
    //    bullet.SetActive(true);

    //    Rigidbody rb = bullet.GetComponent<Rigidbody>();

    //    // reset liner and angular velocity to make the bullet hit right
    //    rb.linearVelocity = Vector3.zero;
    //    rb.angularVelocity = Vector3.zero;

    //    rb.AddForce(bullet.transform.forward * weaponData.bulletForce, ForceMode.Impulse);

    //    Bullet bulletScript = bullet.GetComponent<Bullet>();
    //    if (bulletScript != null)
    //    {
    //        //to change to the data in inventory 
    //        bulletScript.SetDamage(-1/*weaponData.damage*/);
    //        // bulletScript.SetDamage(weaponData.damage);
    //    }

    //    if (muzzleFlash != null)
    //    {
    //        muzzleFlash.Play();
    //    }

    //    if (audioSource && shootSound)
    //    {
    //        audioSource.PlayOneShot(shootSound);
    //    }

    //    targets.Remove(targets[targets.Count - 1]);

    //    if (targets.Count > 0)
    //    {
    //        Debug.Log("double etfo");
    //        StartCoroutine(nady3laetfo(targets));
    //    }

    //    Debug.Log($"targets count {targets.Count}");
    //}


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
    public override IEnumerator ShootForDeadEye(Vector3 targetPosition)
    {
        if (WeaponType == WeaponType.Auto)
        {
            shootDirection = (targetPosition - firePoint.position).normalized;
            firePoint.rotation = Quaternion.LookRotation(shootDirection);
            deadEyeBool = true;
            // Shoot
            Shoot(targetPosition);

            // Longer delay than normal firing rate for DeadEye
            //yield return new WaitForSecondsRealtime(0.25f);
        }
        else
        {
            // Skip IK wait if IKHandler is missing
            //if (ikHandler == null)
            //{
            //    Shoot(targetPosition);
            //    yield break;
            //}

            //Trigger IK aiming
            //ikHandler.TriggerShootIK();
            //shootDirection = (targetPosition - firePoint.position).normalized;
            //firePoint.rotation = Quaternion.LookRotation(shootDirection);

            float maxWaitTime = 0.5f;
            float timer = 0f;

            //while (ikHandler.rig.weight < 0.8f && timer < maxWaitTime)
            //{
            //    timer += Time.unscaledDeltaTime;
            //    yield return null;
            //}

            // Proceed with shooting even if IK isn't perfectly aligned
            deadEyeBool = true;
            Shoot(targetPosition);

            // Small delay between shots (adjust as needed)
            yield return new WaitForSecondsRealtime(1f); // should be changed to the firerate of the weapon 

        }
    }
}