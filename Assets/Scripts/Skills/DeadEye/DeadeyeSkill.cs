using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class DeadeyeSkill : MonoBehaviour
{
    [SerializeField] private float slowMotionFactor = 0; // Factor by which time will be slowed down

    [SerializeField] private IKHandler ikHandler;


    public bool canShoot = true;

    private float duration;
    private float timer;
    private float cooldownTime;
    private float lastUsedTime;

    //private List<GameObject> markedTargets = new List<GameObject>();
    private List<Transform> markedTargets = new List<Transform>();
    [SerializeField] private Transform[] targetsImages;

    private bool canUseAbility;
    private bool isUsingAbility;

    [SerializeField] private Image deadEyeCooldownImage;
    [SerializeField] private Image targetImage;
    [SerializeField] private GameObject parentPanel;

    private PlayerInventory playerInventory;

    //player and else should subscribe to this event
    public event Action OnDeadeyeEffectEnded;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerInventory = player.GetComponent<PlayerInventoryHolder>()?.Inventory;

        //init stats
        //cooldownTime = playerInventory.getPlayerStat(PlayerSkillsStats.DeadEyeCoolDown);
        cooldownTime = 3f;
        duration = 10f;
        lastUsedTime = Time.time - cooldownTime;

        canShoot = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.time - lastUsedTime >= cooldownTime) {
            canUseAbility = true;
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

            UpdateTargetsImages();
        }


    }
    public void UseDeadeye()
    {
        //if (!canUseAbility) return;

        // for future
        //UpdateStats();



        if (Time.time - lastUsedTime >= cooldownTime)
        {
            canUseAbility = false;
            markedTargets.Clear();

            timer = 0;

            lastUsedTime = Time.time;
            //TODO: Implement the logic for using the Deadeye skill
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

    private void UpdateStats()
    {
        // for future
        duration = playerInventory.getPlayerStat(PlayerSkillsStats.DeadEyeDuration);
        cooldownTime = playerInventory.getPlayerStat(PlayerSkillsStats.DeadEyeCoolDown);
    }

    private System.Collections.IEnumerator DeadeyeEffectCoroutine()
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
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
        // each tap
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

            tap.transform.parent = hit.collider.transform; // Attach to enemy
            markedTargets.Add(tap.transform); // Track this anchor for UI updates


            // Create a temporary GameObject at the hit point and parent it to the enemy (can be used for visual markers or debugging)
            GameObject tmpImage = new();
            tmpImage.transform.position = hit.point;
            tmpImage.transform.parent = hit.transform;
        }
    }

    private void UpdateTargetsImages()
    {
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

            // Clean up any destroyed markers from the list
            markedTargets.RemoveAll(marker => marker == null);
        }
    }

    private void TerminateEnemies()
    {
        // uncomment this when the current weapon is done and add a serialize field for it 
        
    }
}