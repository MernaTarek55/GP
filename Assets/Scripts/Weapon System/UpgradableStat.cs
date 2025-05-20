using UnityEngine;

public enum UpgradableStatType
{
    Damage,
    FireRate,
    AreaOfEffectRadius
}
[System.Serializable]
public class UpgradableStat
{
    public UpgradableStatType statType;

    public float baseValue;
    public float maxValue;
    public int maxLevel;

    public AnimationCurve costPerLevel;
    public float GetValue(int level)
    {
        float t = Mathf.Clamp01(level / (float)maxLevel);
        return Mathf.Lerp(baseValue, maxValue, t);
    }
    public int GetCost(int level) => Mathf.RoundToInt(costPerLevel.Evaluate(level));
}
