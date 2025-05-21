using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class ShopUIEditor : EditorWindow
{
    [MenuItem("Tools/Generate Shop UI")]
    public static void GenerateShopUI()
    {
        ShopItem[] allItems = Resources.LoadAll<ShopItem>("ShopItems");

        Dictionary<string, List<ShopItem>> tabGroups = new();

        foreach (var item in allItems)
        {
            string tabName = GetTabName(item);
            if (!tabGroups.ContainsKey(tabName))
                tabGroups[tabName] = new List<ShopItem>();

            tabGroups[tabName].Add(item);
        }

        // Find the ShopPanel in the scene
        var shopPanel = GameObject.Find("ShopPanel");
        if (shopPanel == null)
        {
            Debug.LogError("ShopPanel not found in scene.");
            return;
        }

        Transform tabContainer = shopPanel.transform.Find("TabPagesContainer");
        if (tabContainer == null)
        {
            Debug.LogError("TabPagesContainer not found in ShopPanel.");
            return;
        }
        Transform tabButtons = shopPanel.transform.Find("TabButtonContainer");
        if (tabButtons == null)
        {
            Debug.LogError("TabButtonContainer not found in ShopPanel.");
            return;
        }
        GameObject itemUIPrefab = Resources.Load<GameObject>("ShopItems");

        if (itemUIPrefab == null)
        {
            Debug.LogError("Missing ShopItemUI prefab in Resources.");
            return;
        }

        // Clear previous
        foreach (Transform child in tabContainer)
            DestroyImmediate(child.gameObject);

        foreach (Transform child in tabButtons)
            DestroyImmediate(child.gameObject);

        foreach (var kvp in tabGroups)
        {
            string tabName = kvp.Key;
            List<ShopItem> items = kvp.Value;

            // Create Tab Page
            GameObject tabPage = new GameObject($"Tab_{tabName}", typeof(RectTransform), typeof(CanvasRenderer), typeof(VerticalLayoutGroup));
            tabPage.transform.SetParent(tabContainer, false);

            // Add UI Items
            foreach (var item in items)
            {
                var ui = PrefabUtility.InstantiatePrefab(itemUIPrefab, tabPage.transform) as GameObject;
                var shopItemUI = ui.GetComponent<ShopItemUI>();
                shopItemUI.Bind(item);
            }

            // Create Tab Button
            GameObject tabButton = new GameObject($"TabButton_{tabName}", typeof(Button), typeof(Text));
            tabButton.transform.SetParent(tabButtons, false);
            tabButton.GetComponentInChildren<Text>().text = tabName;
            // (You can hook up OnClick logic here)
        }

        Debug.Log("Shop UI Generated.");
    }

    static string GetTabName(ShopItem item)
    {
        if (item is WeaponItem weapon)
            return weapon.weaponType.ToString();
        else if (item is WeaponUpgradeItem upgrade)
        {
                return upgrade.weaponType.ToString(); // Stat upgrades per weapon tab
        }
        else if(item is PlayerSkillItem || item is HealthItem)
            return "SkillsAndHealth"; // General player upgrades
        return "Misc";
    }
}
