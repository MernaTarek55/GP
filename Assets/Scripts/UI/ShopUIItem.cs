using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Image icon;
    public TextMeshProUGUI costText;

    [SerializeField] private Button buyButton;
    private ShopItem currentItem;

    public void Bind(ShopItem item)
    {
        currentItem = item;
        costText.text = $"Cost: {item.GetCost()}";
    }

    public void OnBuyButtonClicked()
    {
        // Call ShopManager or Inventory to purchase
    }
}
