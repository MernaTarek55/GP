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
        else if (!isUsingAbility && !isExcutingTargets)
        {
            TerminateEnemies();
        }
        if (!isExcutingTargets)
        {
            UpdateTargetsImages();
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
            Time.timeScale = slowMotionFactor;
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
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        isUsingAbility = false; // ✅ Ensure it resets here
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

        GameObject tap = new();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            tap.transform.position = hit.point;
        }
        else
        {
            tap.transform.position = ray.origin + ray.direction * 100f;
        }

        if (hit.collider.gameObject.CompareTag("Enemy"))
        {
            tap.transform.parent = hit.collider.transform;
            markedTargets.Add(tap.transform);

            GameObject tmpImage = new();
            tmpImage.transform.position = hit.point;
            tmpImage.transform.parent = hit.transform;
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

    private void TerminateEnemies()
    {
        GameObject weaponGO = currentWeapon.GetCurrentWeapon();
        Weapon weapon = weaponGO.GetComponent<Weapon>();

        if (weapon == null)
        {
            Debug.LogError("Current weapon does not have a Weapon-derived script attached!");
            return;
        }

        isExcutingTargets = true;
        StartCoroutine(ShootEnemiesSequentially(weapon));
    }

    private IEnumerator ShootEnemiesSequentially(Weapon weapon)
    {
        for (int i = 0; i < markedTargets.Count; i++)
        {
            Transform target = markedTargets[i];
            if (target == null) continue;

            weapon.Shoot(target.position);


            if (i < markedTargets.Count && markedTargets[i] != null)
            {
                targetsImages[i].position = Camera.main.WorldToScreenPoint(markedTargets[i].position);
            }

            Debug.Log("Shooting at marked enemy " + i);
            yield return new WaitForSeconds(weapon.GetFireRate());
            
            if (i < targetsImages.Length)
            {
                targetsImages[i].gameObject.SetActive(false);
            }
        }

        markedTargets.Clear();
        isExcutingTargets = false;
    }
}
