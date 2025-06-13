using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Player Skill Item")]
public class PlayerSkillItem : ShopItem
{
    public PlayerSkillsStats skill;
    public float amountToIncrease = 1f;
    public float amountToDecrease = -1f;

    public override void OnPurchase(PlayerInventory inventory)
    {
        float currentValue = inventory.getPlayerStat(skill);

        // Determine the amount based on the skill
        float amountToApply;

        if (skill == PlayerSkillsStats.InvisibilityCoolDown || skill == PlayerSkillsStats.DeadEyeCoolDown)
        {
            amountToApply = amountToDecrease; // For cooldowns, we want to reduce the value
        }
        else
        {
            amountToApply = amountToIncrease; // For other stats, we increase
        }

        inventory.SetPlayerStat(skill, currentValue + amountToApply);

        // Find the player's InvisibilitySkill and update its stats
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var skillComponent = player.GetComponent<InvisibilitySkill>();
            if (skillComponent != null)
            {
                skillComponent.UpdateStatsFromInventory();
            }
        }
    }
}

