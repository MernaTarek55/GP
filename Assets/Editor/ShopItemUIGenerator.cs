using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Shop UI From ShopItems")]
    public static void GenerateUI()
    {
        ShopItem[] shopItems = Resources.LoadAll<ShopItem>("ShopItems");
        GameObject itemUIPrefab = Resources.Load<GameObject>("ShopItemUITemplet");
        Debug.Log($"Found {shopItems.Length} shop items");
        if (itemUIPrefab == null)
        {
            Debug.LogError("ShopItemUI prefab not found");
            return;
        }
        string outputPath = "Assets/Resources/ShopItemUIPrefabs";
        if (!AssetDatabase.IsValidFolder(outputPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "ShopItemUIPrefabs");
        }

        foreach (ShopItem item in shopItems)
        {
            
            Debug.Log($"Generating UI for {item.name}");
            GameObject uiGO = (GameObject)PrefabUtility.InstantiatePrefab(itemUIPrefab);
            ShopItemUI shopItemUI = uiGO.GetComponent<ShopItemUI>();
            if (shopItemUI == null)
            {
                Debug.LogWarning("ShopItemUI prefab does not contain ShopItemUI script.");
                GameObject.DestroyImmediate(uiGO);
                continue;
            }

            string displayName = GetDisplayNameFromAsset(item);
            //TODO
            //Sprite icon = LoadIconForItem(displayName);

            shopItemUI.Bind(item);

            string filename = $"{item.name}_ShopItemUI.prefab";
            if(item.name.Contains("Weapon"))
            {
                Button btn = uiGO.GetComponent<Button>();
                Image img = uiGO.GetComponent<Image>();

                if (img != null)
                {
                    GameObject.DestroyImmediate(img);
                    GameObject.DestroyImmediate(btn);
                }
                //delete word from string
                displayName = displayName.Replace("Weapon", "").Trim();
            }
            shopItemUI.itemNameText.text = displayName;
            PrefabUtility.SaveAsPrefabAsset(uiGO, Path.Combine(outputPath, filename));
            GameObject.DestroyImmediate(uiGO);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("ShopItemUI prefabs generated in Resources/ShopItemUIPrefabs");
    }

    private static Sprite LoadIconForItem(string itemName)
    {
        return Resources.Load<Sprite>($"Icons/{itemName}") ?? Resources.Load<Sprite>("Icons/Default");
    }
    private static string GetDisplayNameFromAsset(ShopItem item)
    {
        string path = AssetDatabase.GetAssetPath(item);
        string filename = System.IO.Path.GetFileNameWithoutExtension(path);

        //eg: Upgrade_Gun_Damage_ShopItem
        // -> "Gun Damage Upgrade"
        string[] tokens = filename.Split('_');

        if (tokens.Length >= 3 && tokens[0] == "Upgrade")
        {
            string weapon = tokens[1];
            string stat = tokens[2];
            return $"{weapon} {stat} Upgrade";
        }
        else if (tokens.Length >= 2 && tokens[0] == "Weapon")
        {
            string weapon = tokens[1];
            return $"{weapon} Weapon";
        }
        else if (tokens.Length >= 2 && tokens[0] == "Skill")
        {
            string skill = tokens[1];
            return $"{skill} Upgrade";
        }
        else if (tokens.Length >= 2 && tokens[0] == "Health")
        {
            string healthType = tokens[1];
            return $"{healthType} Health Upgrade";
        }
        else
        {
            // Fallback to raw filename with spaces
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                filename.Replace('_', ' ').ToLower());
        }
    }

}
