using UnityEngine;

public class shopButton : MonoBehaviour
{
    [SerializeField]GameObject shopPanel;

    public void ToggleShop()
    {
        if (shopPanel == null)
        {
            shopPanel = GameObject.Find("ShopRootPanel");
        }
        if (shopPanel != null)
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
        }
        else
        {
            Debug.LogError("Shop panel not found!");
        }
    }
}
