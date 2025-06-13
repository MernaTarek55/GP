using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIEditorGenerator : EditorWindow
{
    private static readonly List<Button> allTabButtons = new();
    private static readonly List<GameObject> allTabPanels = new();

    [MenuItem("Tools/Generate Shop UI")]
    public static void GenerateShopUI()
    {
        // Check or create Canvas
        Canvas canvas = null;
        if (canvas == null)
        {
            GameObject canvasGO = new("ShopCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Create root panel for shop UI
        GameObject rootPanel = new("ShopRootPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        rootPanel.transform.SetParent(canvas.transform, false);
        Image img = rootPanel.GetComponent<Image>();
        img.color = new Color(0, 0, 0, 0.5f);
        RectTransform rootRect = rootPanel.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = rootRect.offsetMax = Vector2.zero;

        // Create tab buttons container
        GameObject tabButtonsPanel = new("TabButtonsPanel", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        tabButtonsPanel.transform.SetParent(rootPanel.transform, false);
        HorizontalLayoutGroup tabLayout = tabButtonsPanel.GetComponent<HorizontalLayoutGroup>();
        tabLayout.childForceExpandWidth = false;
        tabLayout.childForceExpandHeight = false;
        RectTransform tabButtonsRect = tabButtonsPanel.GetComponent<RectTransform>();
        tabButtonsRect.anchorMin = new Vector2(0, 1);
        tabButtonsRect.anchorMax = new Vector2(1, 1);
        tabButtonsRect.pivot = new Vector2(0.5f, 1);
        tabButtonsRect.sizeDelta = new Vector2(0, 40);
        tabButtonsRect.anchoredPosition = Vector2.zero;

        // Create container for tab content panels
        GameObject tabContentContainer = new("TabContentContainer", typeof(RectTransform));
        tabContentContainer.transform.SetParent(rootPanel.transform, false);
        RectTransform contentRect = tabContentContainer.GetComponent<RectTransform>();

        TabsManager tm = tabContentContainer.AddComponent<TabsManager>();
        tm.tabContentContainer = contentRect;


        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = new Vector2(0, 0);
        contentRect.offsetMax = new Vector2(0, -80);

        // Load all ShopItemUI prefabs
        ShopItemUI[] allShopItemUIs = Resources.LoadAll<ShopItemUI>("ShopItemUIPrefabs");
        if (allShopItemUIs == null || allShopItemUIs.Length == 0)
        {
            Debug.LogError("No ShopItemUI prefabs found in Resources/ShopItemUIPrefabs");
            return;
        }

        // Categorize by tab
        Dictionary<string, List<ShopItemUI>> categorized = new();

        foreach (ShopItemUI itemUI in allShopItemUIs)
        {
            string tabKey = GetTabKeyFromName(itemUI.name);

            Debug.Log($"Creating new category: {tabKey}");
            if (!categorized.ContainsKey(tabKey))
            {
                categorized[tabKey] = new List<ShopItemUI>();
            }

            categorized[tabKey].Add(itemUI);
        }

        // For each category: create tab button & content panel & add item UIs
        foreach (KeyValuePair<string, List<ShopItemUI>> kvp in categorized)
        {
            string tabName = kvp.Key;
            List<ShopItemUI> items = kvp.Value;

            // Create tab button
            GameObject tabButton = new($"TabButton_{tabName}", typeof(RectTransform), typeof(Button), typeof(Text));
            tabButton.transform.SetParent(tabButtonsPanel.transform, false);

            Text btnText = tabButton.GetComponent<Text>();
            btnText.text = tabName;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.color = Color.black;

            // Create tab content panel
            GameObject tabPanel = new($"TabPanel_{tabName}", typeof(RectTransform), typeof(CanvasGroup));
            tabPanel.transform.SetParent(tabContentContainer.transform, false);
            RectTransform tabRect = tabPanel.GetComponent<RectTransform>();
            tabRect.anchorMin = Vector2.zero;
            tabRect.anchorMax = Vector2.one;
            tabRect.offsetMin = Vector2.zero;
            tabRect.offsetMax = Vector2.zero;

            allTabButtons.Add(tabButton.GetComponent<Button>());
            allTabPanels.Add(tabPanel);

            tm.tabs.Add(new TabsManager.TabData
            {
                tabButton = tabButton.GetComponent<Button>(),
                tabPanel = tabPanel
            });
            // Add layout group to tab panel for items
            VerticalLayoutGroup layout = tabPanel.AddComponent<VerticalLayoutGroup>();
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 55;

            // Instantiate each ShopItemUI prefab under tabPanel
            foreach (ShopItemUI itemUI in items)
            {
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(itemUI.gameObject);
                go.transform.SetParent(tabPanel.transform, false);
            }

            // Optionally disable all tabs except first
            if (tabName != "Weapons")
            {
                tabPanel.SetActive(false);
            }

            // TODO: Add tab button click logic to toggle tab panels
            Button button = tabButton.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                Debug.Log($"Switching to tab: {tabName}");
                foreach (Transform child in tabContentContainer.transform)
                {
                    child.gameObject.SetActive(false);
                }
                tabPanel.SetActive(true);
            });

        }
        TabsManager tabsManager = rootPanel.AddComponent<TabsManager>();
        tabsManager.tabContentContainer = tabContentContainer.GetComponent<RectTransform>();

        for (int i = 0; i < allTabButtons.Count; i++)
        {
            TabsManager.TabData data = new()
            {
                tabButton = allTabButtons[i],
                tabPanel = allTabPanels[i]
            };
            tabsManager.tabs.Add(data);
        }

        Debug.Log("Shop UI generated in scene");
    }

    private static string GetTabKeyFromName(string name)
    {
        // Example: "ShopItemUI_Upgrade_Sniper_Damage" -> "Sniper"
        // Or fallback to generic categories like Weapons, Skills, Health
        if (name.Contains("Upgrade"))
        {
            return name.Split("_")[1];
        }
        else if (name.Contains("Weapon"))
        {
            return "Weapons";
        }
        else if (name.Contains("Skill"))
        {
            return "Skills";
        }
        else if (name.Contains("Health"))
        {
            return "Health";
        }

        return "Misc";
    }
}
