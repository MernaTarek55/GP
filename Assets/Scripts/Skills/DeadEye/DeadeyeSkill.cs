using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadeyeSkill : MonoBehaviour
{
    [SerializeField] private float slowMotionFactor = 0;
    [SerializeField] private IKHandler ikHandler;
    [SerializeField] PlayerInventoryHolder playerInventoryHolder;

    private float timer;
    private float lastUsedTime;
    private bool isUsingAbility = false;
    //getter and setter for isUsingAbility
    public bool IsUsingAbility
    {
        get { return isUsingAbility; }
        private set { }
    }

    private List<Transform> markedTargets = new List<Transform>();
    [SerializeField] private Transform[] targetsImages;

    private PlayerInventory playerInventory;
    [SerializeField] WeaponSwitch weapons;

    private void Start()
    {
        playerInventory = playerInventoryHolder.Inventory;
    }

    private void Update()
    {

        if (isUsingAbility)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector2 touchPos = Input.GetTouch(0).position;
                AddTapPosition(touchPos);
            }
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
        if (Time.time - lastUsedTime >= playerInventory.getPlayerStat(PlayerSkillsStats.DeadEyeCoolDown))
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
        Time.timeScale = slowMotionFactor;

        yield return new WaitForSecondsRealtime(playerInventory.getPlayerStat(PlayerSkillsStats.DeadeyeDuration));

        // Reset time scale when DeadEye ends
        Time.timeScale = 1f;
        isUsingAbility = false;
        Debug.Log("Deadeye effect ended.");
        ShootTargets();
    }

    private void ShootTargets()
    {
        for (int i = 0; i < markedTargets.Count; i++)
        {
            weapons.GetCurrentWeapon().ShootForDeadEye(markedTargets[i].position);
            
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
        int i = 0;
        doneWithLastTargets = false;
        foreach (Transform target in targetsToShoot)
        {
            if (target == null) continue;
            Debug.Log("here for "+i);
            i++;
            // Use StartCoroutine and yield return to wait properly
            yield return StartCoroutine(weapon.ShootForDeadEye(target.position));

            // Optional: Update UI
            UpdateTargetsImagesforAfterDeadeye();
        }
        doneWithLastTargets = true;
        Debug.Log("Deadeye Done");

        // Clear only after all shots are done
        markedTargets.Clear();
        isExcutingTargets = false;
        
    }
}