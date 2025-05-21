#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public static class ShopItemGenerator
{
    [MenuItem("Tools/Generate Shop Items")]
    public static void GenerateShopItems()
    {
        string outputPath = "Assets/Resources/ShopItems";

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        //foreach (var asset in Directory.GetFiles(outputPath, "*.asset"))
        //    AssetDatabase.DeleteAsset(asset);

        var allWeapons = Resources.LoadAll<WeaponData>("WeaponData");

        foreach (WeaponData weapon in allWeapons)
        {
            WeaponItem weaponItem = ScriptableObject.CreateInstance<WeaponItem>();
            weaponItem.weaponType = weapon.weaponType;

            //weaponItem.itemName = $"Buy {weapon.weaponType}";
            //weaponItem.baseCost = weapon.baseCost;
            //weaponItem.icon = weapon.icon;

            AssetDatabase.CreateAsset(weaponItem, $"{outputPath}/Weapon_{weapon.weaponType}_ShopItem.asset");

            //Weapon Upgrade Shop Items
            foreach (UpgradableStat stat in weapon.upgradableStats)
            {
                WeaponUpgradeItem upgradeItem = ScriptableObject.CreateInstance<WeaponUpgradeItem>();
                upgradeItem.weaponType = weapon.weaponType;
                upgradeItem.statToUpgrade = stat.statType;

                //upgradeItem.itemName = $"{weapon.weaponType} - Upgrade {stat.statType}";
                //upgradeItem.baseCost = stat.costPerLevel[0];
                //upgradeItem.icon = weapon.icon;

                AssetDatabase.CreateAsset(upgradeItem, $"{outputPath}/Upgrade_{weapon.weaponType}_{stat.statType}_ShopItem.asset");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Shop items generated successfully.");
    }
}
#endif
