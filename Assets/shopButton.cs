using UnityEngine;

public class shopButton : MonoBehaviour
{
    GameObject shopPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
