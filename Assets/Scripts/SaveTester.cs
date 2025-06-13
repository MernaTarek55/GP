using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class SaveTester : MonoBehaviour
{
    PlayerInventory playerInventory;
    InventorySaveManager saveManager;
    InventorySaveData saveData;
    [SerializeField] PlayerInventoryHolder playerInventoryHolder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!GameStartType.IsNewGame)
        {
            playerInventory = playerInventoryHolder.Inventory;
            saveData = playerInventory.inventorySaveData;
            saveManager = new InventorySaveManager(saveData.ownedWeapons, saveData.weaponUpgrades, saveData.bulletsCount, saveData.playerStats, saveData.credits);

            InventorySaveData data = saveManager.Load();
            if (data != null) data.printData();
            else Debug.Log("No save data found, initializing new inventory.");
            if (data != null)
            {
                playerInventory.Credits = data.credits;
                saveData.ownedWeapons.Clear();
                foreach (var w in data.ownedWeapons) saveData.ownedWeapons.Add(w);
                foreach (var kv in data.weaponUpgrades) saveData.weaponUpgrades[kv.Key] = kv.Value;
                foreach (var kv in data.bulletsCount) saveData.bulletsCount[kv.Key] = kv.Value;
                foreach (var kv in data.playerStats) saveData.playerStats[kv.Key] = kv.Value;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S pressed, saving game");
            SaveGame();
        }
    }
    public void SaveGame()
    {
        saveManager = new InventorySaveManager(saveData.ownedWeapons, saveData.weaponUpgrades, saveData.bulletsCount, saveData.playerStats, saveData.credits);
        saveManager.Save();
    }
}
