using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

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
            AssetDatabase.CreateFolder("Assets/Resources", "ShopItemUIPrefabs");

        foreach (ShopItem item in shopItems)
        {
            Debug.Log($"Generating UI for {item.name}");
            GameObject uiGO = (GameObject)PrefabUtility.InstantiatePrefab(itemUIPrefab);
            ShopItemUI shopItemUI = uiGO.GetComponent<ShopItemUI>();
            if (shopItemUI == null)
            {
                Debug.LogError("ShopItemUI prefab does not contain ShopItemUI script.");
                GameObject.DestroyImmediate(uiGO);
                continue;
            }

            string displayName = GetDisplayNameFromAsset(item);
            //TODO
            //Sprite icon = LoadIconForItem(displayName);

            shopItemUI.itemNameText.text = displayName;
            shopItemUI.Bind(item);

            string filename = $"ShopItemUI_{item.name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(uiGO, Path.Combine(outputPath, filename));
            GameObject.DestroyImmediate(uiGO);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("ShopItemUI prefabs generated in Resources/ShopItemUIPrefabs");
    }

    static Sprite LoadIconForItem(string itemName)
    {
        return Resources.Load<Sprite>($"Icons/{itemName}") ?? Resources.Load<Sprite>("Icons/Default");
    }
    static string GetDisplayNameFromAsset(ShopItem item)
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
        //TODO : if case for health and skills
        else
        {
            // Fallback to raw filename with spaces
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                filename.Replace('_', ' ').ToLower());
        }
    }

}
