using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Singleton;
    PlayerInventory playerInventory;
    InventorySaveManager saveManager;
    InventorySaveData saveData;
    [SerializeField] public PlayerInventoryHolder playerInventoryHolder;
    public int LastPlayedLevel = 0;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInventory = playerInventoryHolder.Inventory;
        saveData = playerInventory.inventorySaveData;
        
    }
    public void MakeGameReady()
    {
        if (!GameStartType.IsNewGame)
        {
            saveManager = new InventorySaveManager(
                saveData.ownedWeapons,
                saveData.weaponUpgrades,
                saveData.bulletsCount,
                saveData.playerStats,
                saveData.credits,
                saveData.lastPlayedLevelIndex
            );
            LoadGame();
            if (saveData != null)
            {
                playerInventory.Credits = saveData.credits;
                LastPlayedLevel = saveData.lastPlayedLevelIndex;
            }
        }
        else
        {
            Debug.Log("why");
            saveData = new InventorySaveData();
            saveData.lastPlayedLevelIndex = LastPlayedLevel;
            playerInventory.inventorySaveData = saveData;
            saveManager = new InventorySaveManager(
                saveData.ownedWeapons,
                saveData.weaponUpgrades,
                saveData.bulletsCount,
                saveData.playerStats,
                saveData.credits,
                saveData.lastPlayedLevelIndex
            );
            
        }
    }

    // Update is called once per frame
    void OnApplicationQuit()
    {
        Debug.Log("Application is quitting, saving game...");
        SaveGame();
    }
    public void SaveGame()
    {
        saveData.lastPlayedLevelIndex = LastPlayedLevel;
        saveManager = new InventorySaveManager(
            saveData.ownedWeapons,
            saveData.weaponUpgrades,
            saveData.bulletsCount,
            saveData.playerStats,
            saveData.credits,
            saveData.lastPlayedLevelIndex
        );
        saveManager.Save();
    }
    public void LoadGame()
    {
        saveData = saveManager.Load();
    }
}
