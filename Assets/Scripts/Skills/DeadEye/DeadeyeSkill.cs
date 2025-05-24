using System;
using UnityEngine;

public class DeadeyeSkill : MonoBehaviour
{
    [SerializeField] private float slowMotionFactor = 0.5f; // Factor by which time will be slowed down
    private float duration;
    private float cooldownTime;
    private float lastUsedTime;
    private PlayerInventory playerInventory;

    //player and else should supscribe to this event
    public event Action OnDeadeyeEffectEnded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var player = GameObject.FindWithTag("Player");
        playerInventory = player.GetComponent<PlayerInventoryHolder>()?.Inventory;

        //init stats
        cooldownTime = playerInventory.getPlayerStat(PlayerSkillsStats.DeadEyeCoolDown);
        lastUsedTime = Time.time - cooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UseDeadeye()
    {
        UpdateStats();
        

        if (Time.time - lastUsedTime >= cooldownTime)
        {
            lastUsedTime = Time.time;
            //TODO: Implement the logic for using the Deadeye skill
            Debug.Log("Deadeye skill used!");
            Time.timeScale = slowMotionFactor;
            StartCoroutine(DeadeyeEffectCoroutine());
        }
        else
        {
            Debug.Log("Deadeye skill is on cooldown.");
        }
    }

    private void UpdateStats()
    {
        duration = playerInventory.getPlayerStat(PlayerSkillsStats.DeadeyeDuration);
        cooldownTime = playerInventory.getPlayerStat(PlayerSkillsStats.DeadEyeCoolDown);
    }

    private System.Collections.IEnumerator DeadeyeEffectCoroutine()
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        OnDeadeyeEffectEnded?.Invoke();
        Debug.Log("Deadeye effect ended.");
    }


}
