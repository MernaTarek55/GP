using UnityEngine;
using UnityEngine.UI;

public abstract class ShopItem : ScriptableObject
{
    public string itemName;
    public int cost;
    public Image icon;
    public string description;

}

[CreateAssetMenu(menuName = "Shop/Weapon Item")]
public class WeaponItem : ShopItem
{
    //Wepon
}

[CreateAssetMenu(menuName = "Shop/Upgrade Item")]
public class UpgradeItem : ShopItem
{
    ItemEffect effect;
}
