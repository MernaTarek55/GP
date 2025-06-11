using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvisibilitySkill : MonoBehaviour
{
    public SkinnedMeshRenderer[] renderers;
    public MeshRenderer[] Weaponrenderers;
    private Material[] materials;
    //need refactor
    private readonly float duration;
    private float cooldownTime;
    public float invisibilityDuration = 30f;
    public float cooldownDuration = 10f;
    //private float lastUsedTime;
    public float fadeSpeed = 2f;
    private PlayerInventory playerInventory;
    public bool isInvisible = false;
    private bool isOnCooldown = false;
    public Button InvisibleButton;
    private void Awake()
    {
        List<Material> matsList = new();
        foreach (SkinnedMeshRenderer r in renderers)
        {
            matsList.AddRange(r.materials);
        }
        foreach (MeshRenderer r in Weaponrenderers)
        {
            matsList.AddRange(r.materials);
        }
        materials = matsList.ToArray();
    }
    private void Start()
    {
        var holder = GameObject.FindWithTag("Player").GetComponent<PlayerInventoryHolder>();
        if (holder == null)
        {
            Debug.LogError("PlayerInventoryHolder not found on player!");
            return;
        }

        playerInventory = holder.Inventory;

        // Load the stats from inventory
        UpdateStatsFromInventory();
        //lastUsedTime = Time.time - cooldownTime;
    }
    public void UpdateStatsFromInventory()
    {
        cooldownDuration = playerInventory.getPlayerStat(PlayerSkillsStats.InvisibilityCoolDown);
        invisibilityDuration = playerInventory.getPlayerStat(PlayerSkillsStats.InvisibilityDuration);

        Debug.Log($"Invisibility Skill Updated - Duration: {invisibilityDuration}, Cooldown: {cooldownDuration}");
    }
    public void UseInvisibility()
    {
        //UpdateStats();
        if (!isInvisible && !isOnCooldown)
        {
            InvisibleButton.interactable = false;
            _ = StartCoroutine(BecomeInvisible());
        }
    }
    //private void UpdateStats()
    //{
    //    duration = playerInventory.getPlayerStat(PlayerSkillsStats.InvesabilityDuration);
    //    cooldownTime = playerInventory.getPlayerStat(PlayerSkillsStats.InvesabilityCoolDown);
    //}
    private IEnumerator BecomeInvisible()
    {
        isInvisible = true;
        yield return Fade(0f, 1f);
        yield return new WaitForSeconds(invisibilityDuration);
        yield return Fade(1f, 0f);

        isInvisible = false;
        _ = StartCoroutine(Cooldown());
    }
    private IEnumerator Fade(float from, float to)
    {
        float t = from;
        float direction = Mathf.Sign(to - from);

        while ((direction > 0 && t < to) || (direction < 0 && t > to))
        {
            t += direction * Time.deltaTime * fadeSpeed;
            t = Mathf.Clamp01(t);

            foreach (Material mat in materials)
            {
                mat.SetFloat("_Invisibility", t);
            }

            yield return null;
        }
    }

    private IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
        InvisibleButton.interactable = true;
    }
}
