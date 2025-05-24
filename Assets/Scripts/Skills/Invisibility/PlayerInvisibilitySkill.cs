using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInvisibilitySkill : EntityState
{
    private float fadeSpeed = 2f;

    private float timer;
    private float duration;
    private float cooldown;
    //private float lastUsedTime;
    //private bool isOnCooldown = false;

    private PlayerInventory playerInventory;
    private Image invisibilityCooldownImage;

    public PlayerInvisibilitySkill(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("laaaaaaaaaaaaaaaaaaaaaaaaa");
        timer = 0f;
        Time.timeScale = 1f; // No slow-down for invisibility
        Time.fixedDeltaTime = 0.02f;

        playerInventory = player.GetComponent<PlayerInventoryHolder>()?.Inventory;
        duration = playerInventory.getPlayerStat(PlayerSkillsStats.InvesabilityDuration);
        cooldown = playerInventory.getPlayerStat(PlayerSkillsStats.InvesabilityCoolDown);
       // lastUsedTime = Time.unscaledTime;

        //player.isInvisible = true;
        UpdateCooldownUI(0);
        player.StartCoroutine(Fade(0f, 1f)); // Fade to invisible
        Debug.Log("Invisibility activated!");
    }

    public override void Update()
    {
        base.Update();

        timer += Time.unscaledDeltaTime;
        UpdateCooldownUI(timer / duration);

        if (timer >= duration)
        {
            Exit();
            player.StartCoroutine(Fade(1f, 0f)); // Fade back to visible
            StartCooldown();
            stateMachine.ChangeState(new Player_IdleState(stateMachine, "Idle", player));
        }
    }

    public override void Exit()
    {
        base.Exit();
        //player.isInvisible = false;
        UpdateCooldownUI(0);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;
            foreach (var mat in player.Materials)
            {
                mat.SetFloat("_Invisibility", Mathf.Clamp01(t));
            }
            yield return null;
        }
    }

    private void StartCooldown()
    {
        player.StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        //isOnCooldown = true;
        float cooldownTimer = 0f;

        while (cooldownTimer < cooldown)
        {
            cooldownTimer += Time.unscaledDeltaTime;
            UpdateCooldownUI(1f - (cooldownTimer / cooldown));
            yield return null;
        }

        //isOnCooldown = false;
        UpdateCooldownUI(0);
        Debug.Log("Invisibility cooldown finished");
    }

    private void UpdateCooldownUI(float fillAmount)
    {
        if (invisibilityCooldownImage != null)
            invisibilityCooldownImage.fillAmount = fillAmount;
    }
}
