using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadeyeSkill : MonoBehaviour
{
    [SerializeField] private float slowMotionFactor = 0;

    [SerializeField] private IKHandler ikHandler;
    [SerializeField] private WeaponSwitch currentWeapon;

    public bool canShoot = true;

    private float duration;
    private float timer;
    private float cooldownTime;
    private float lastUsedTime;

    private List<Transform> markedTargets = new List<Transform>();
    [SerializeField] private Transform[] targetsImages;

    private bool isExcutingTargets;
    private bool isUsingAbility;

    [SerializeField] private Image deadEyeCooldownImage;
    [SerializeField] private Image targetImage;
    [SerializeField] private GameObject parentPanel;

    private PlayerInventory playerInventory;

    public event Action OnDeadeyeEffectEnded;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerInventory = player.GetComponent<PlayerInventoryHolder>()?.Inventory;

        cooldownTime = 1f;
        duration = 3f;
        lastUsedTime = Time.time - cooldownTime;

        canShoot = true;
    }

    private void Update()
    {
        if (Time.time - lastUsedTime >= cooldownTime)
        {
            isExcutingTargets = false;
            canShoot = true;
            isUsingAbility = false;
            UpdateCooldownUI(1);
        }

        if (isUsingAbility)
        {
            canShoot = false;

            timer += Time.unscaledDeltaTime;
            UpdateCooldownUI(1 - (timer / duration));

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector2 touchPos = Input.GetTouch(0).position;
                AddTapPosition(touchPos);
            }
        }
        else if (!isUsingAbility && !isExcutingTargets && markedTargets.Count > 0)
        {
            TerminateEnemies();
        }

        if (!isExcutingTargets)
        {
            UpdateTargetsImages();
        }
        else
        {
            UpdateTargetsImagesforAfterDeadeye();
        }
    }

    public void UseDeadeye()
    {
        if (Time.time - lastUsedTime >= cooldownTime)
        {
            markedTargets.Clear();
            timer = 0;
            lastUsedTime = Time.time;

            Debug.Log("Deadeye skill used!");

            // Apply different slow-motion based on weapon type
            GameObject weaponGO = currentWeapon.GetCurrentWeapon();
            Weapon weapon = weaponGO.GetComponent<Weapon>();
            Time.timeScale = (weapon.WeaponType is WeaponType.Auto) ? 0.5f : slowMotionFactor;

            StartCoroutine(DeadeyeEffectCoroutine());
            isUsingAbility = true;
        }
        else
        {
            isUsingAbility = false;
            Debug.Log("Deadeye skill is on cooldown.");
        }
    }

    private IEnumerator DeadeyeEffectCoroutine()
    {
        // Apply slow-motion at the start
        GameObject weaponGO = currentWeapon.GetCurrentWeapon();
        Weapon weapon = weaponGO.GetComponent<Weapon>();
        Time.timeScale = (weapon.WeaponType is WeaponType.Auto) ? 0.5f : slowMotionFactor;

        yield return new WaitForSecondsRealtime(duration);

        // Reset time scale when DeadEye ends
        Time.timeScale = 1f;
        isUsingAbility = false;
        OnDeadeyeEffectEnded?.Invoke();
        Debug.Log("Deadeye effect ended.");
    }

    private void UpdateCooldownUI(float fillAmount)
    {
        if (deadEyeCooldownImage != null)
        {
            deadEyeCooldownImage.fillAmount = fillAmount;
        }
    }

    private void AddTapPosition(Vector2 screenPosition)
    {
        if (markedTargets.Count >= targetsImages.Length)
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                GameObject tap = new GameObject("TargetMarker");
                tap.transform.position = hit.point;
                tap.transform.parent = hit.collider.transform;
                markedTargets.Add(tap.transform);

                GameObject tmpImage = new GameObject("TargetImage");
                tmpImage.transform.position = hit.point;
                tmpImage.transform.parent = hit.transform;
            }
        }
        else
        {
            // Optional: if you want to create a fallback marker in empty space
            GameObject tap = new GameObject("TargetMarker");
            tap.transform.position = ray.origin + ray.direction * 100f;
        }
    }


    private void UpdateTargetsImages()
    {
        markedTargets.RemoveAll(marker => marker == null);

        for (int i = 0; i < targetsImages.Length; i++)
        {
            if (i < markedTargets.Count && markedTargets[i] != null)
            {
                targetsImages[i].gameObject.SetActive(true);
                targetsImages[i].position = Camera.main.WorldToScreenPoint(markedTargets[i].position);
            }
            else
            {
                targetsImages[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateTargetsImagesforAfterDeadeye()
    {

        for (int i = 0; i < targetsImages.Length; i++)
        {
            if (i < markedTargets.Count && markedTargets[i] != null)
            {
                targetsImages[i].position = Camera.main.WorldToScreenPoint(markedTargets[i].position);
            }
        }
    }

    private void TerminateEnemies()
    {
        GameObject weaponGO = currentWeapon.GetCurrentWeapon();
        if (weaponGO == null) // Add null check
        {
            Debug.LogError("No current weapon found!");
            return;
        }

        Weapon weapon = weaponGO.GetComponent<Weapon>();
        if (weapon == null) // Add null check
        {
            Debug.LogError("Current weapon has no Weapon component!");
            return;
        }

        isExcutingTargets = true;
        StartCoroutine(ShootEnemiesSequentially(weapon));
    }
    
    private IEnumerator ShootEnemiesSequentially(Weapon weapon)
    {
        // Create a copy to avoid modification during iteration
        List<Transform> targetsToShoot = new List<Transform>(markedTargets);

        foreach (Transform target in targetsToShoot)
        {
            if (target == null) continue;

            Debug.Log($"Shooting at target: {target.position}");

            // Use StartCoroutine and yield return to wait properly
            yield return StartCoroutine(weapon.ShootForDeadEye(target.position));

            // Optional: Update UI
            UpdateTargetsImagesforAfterDeadeye();
        }

        // Clear only after all shots are done
        markedTargets.Clear();
        isExcutingTargets = false;
        
    }
}