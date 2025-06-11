using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Image icon;
    public TextMeshProUGUI costText;

    [SerializeField] private Button buyButton;
    [SerializeField] private ShopItem currentItem;

    private void Awake()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);

    }
    public void Bind(ShopItem item)
    {
        currentItem = item;
        Debug.Log($"Binding {currentItem.name} to UI");
        costText.text = $"Cost: {currentItem.GetCost()}";
    }

    public void OnBuyButtonClicked()
    {
        if (ShopManager.Singelton.Buy(currentItem))
        {
            UpdateUICost();
        }
        //TODO: sound effect?
    }

    private void UpdateUICost()
    {
        if (currentItem is WeaponUpgradeItem upgradeItem)
        {
            costText.text = $"Cost: {upgradeItem.GetLevelCost(ShopManager.Singelton.playerInventory)}";
        }
    }
}
