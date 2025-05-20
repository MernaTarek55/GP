using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public enum Stats
{
    MaxHealth,
    DeadEyeCoolDown,
    DeadEyeDuration,
    WeponDamage,
    WeponFireRate,
}

public interface ItemEffect
{
    //void ApplyEffect(Player player);
}

[CreateAssetMenu(fileName = "Heal Effect", menuName = "Effects/Heal Effect")]
public class HealEffect : ScriptableObject, ItemEffect
{
    public int healAmount;

}

[CreateAssetMenu(fileName = "Buff Effect", menuName = "Effects/Buff Effect")]
public class BuffEffect : ScriptableObject, ItemEffect
{
    public float amountToBuff;
    public Stats statToBuff;
}
