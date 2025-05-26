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
    private float duration;
    private float cooldownTime;
    public float invisibilityDuration = 10f;
    public float cooldownDuration = 10f;
    private float lastUsedTime;
    public float fadeSpeed = 2f;
    private PlayerInventory playerInventory;
    public bool isInvisible = false;
    private bool isOnCooldown = false;
    public Button InvisibleButton;
    void Awake()
    {
        var matsList = new List<Material>();
        foreach (var r in renderers)
        {
            matsList.AddRange(r.materials);
        }
        foreach (var r in Weaponrenderers)
        {
            matsList.AddRange(r.materials);
        }
        materials = matsList.ToArray();
    }
    private void Start()
    {
        //var player = GameObject.FindWithTag("Player");
        //playerInventory = player.GetComponent<PlayerInventoryHolder>()?.Inventory;
        //cooldownTime = playerInventory.getPlayerStat(PlayerSkillsStats.InvesabilityCoolDown);
        lastUsedTime = Time.time - cooldownTime;
    }
    public void UseInvisibility()
    {
        //UpdateStats();
        if (!isInvisible && !isOnCooldown)
        {
            InvisibleButton.interactable = false;
            StartCoroutine(BecomeInvisible());
        }
    }
    //private void UpdateStats()
    //{
    //    duration = playerInventory.getPlayerStat(PlayerSkillsStats.InvesabilityDuration);
    //    cooldownTime = playerInventory.getPlayerStat(PlayerSkillsStats.InvesabilityCoolDown);
    //}
    IEnumerator BecomeInvisible()
    {
        isInvisible = true;
        yield return Fade(0f, 1f);
        yield return new WaitForSeconds(invisibilityDuration);
        yield return Fade(1f, 0f);

        isInvisible = false;
        StartCoroutine(Cooldown());
    }
    IEnumerator Fade(float from, float to)
    {
        float t = from;
        float direction = Mathf.Sign(to - from);

        while ((direction > 0 && t < to) || (direction < 0 && t > to))
        {
            t += direction * Time.deltaTime * fadeSpeed;
            t = Mathf.Clamp01(t);

            foreach (var mat in materials)
            {
                mat.SetFloat("_Invisibility", t);
            }

            yield return null;
        }
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
        InvisibleButton.interactable = true;
    }
}
