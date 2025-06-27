using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Player Skill Item")]
public class PlayerSkillItem : ShopItem
{
    public PlayerSkillsStats skill;
    [SerializeField] float amountToChange;
    public float maxVal;

    public override void OnPurchase(PlayerInventory inventory)
    {
        float currentValue = inventory.getPlayerStat(skill);

        inventory.SetPlayerStat(skill, currentValue + amountToChange);

        // Find the player's InvisibilitySkill and update its stats
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var skillComponent = player.GetComponent<InvisibilitySkill>();
            if (skillComponent != null)
            {
                //skillComponent.UpdateStatsFromInventory();
            }
        }
    }
}

