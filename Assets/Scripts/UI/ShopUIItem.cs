using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Sprite icon;
    public TextMeshProUGUI costText;

    [SerializeField] private Button buyButton;
    [SerializeField] private ShopItem currentItem;

    private void Awake()
    {
        if(buyButton != null)
            buyButton.onClick.AddListener(OnBuyButtonClicked);

    }
    public void Bind(ShopItem item)
    {
        currentItem = item;
        costText.text = $"Cost: {currentItem.GetCost()}";
    }
    public int GetItemCost() 
    {
        if (currentItem is WeaponUpgradeItem upgradeItem)
        {
            return upgradeItem.GetLevelCost(ShopManager.Singelton.playerInventory);
        }
        return currentItem.GetCost();
    }
    public string GetItemName()
    {
        return itemNameText.text;
    }
    public bool IsFullyBought()
    {
        return currentItem.isOwned;
    }
    public void OnBuyButtonClicked()
    {
        if (ShopManager.Singelton.Buy(currentItem))
        {
            UpdateUICost();
        }
        //TODO: sound effect?
    }

    public void UpdateUICost()
    {
        if (currentItem is WeaponUpgradeItem upgradeItem)
        {
            costText.text = $"Cost: {GetItemCost()}";
        }
        else
        {
            costText.text = "";
        }
    }
}
