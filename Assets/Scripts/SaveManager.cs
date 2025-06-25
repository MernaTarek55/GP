
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Singleton;
    PlayerInventory playerInventory;
    InventorySaveManager saveManager;
    InventorySaveData saveData;
    [SerializeField] public PlayerInventoryHolder playerInventoryHolder;

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
        if (!GameStartType.IsNewGame)
        {
            saveManager = new InventorySaveManager(saveData.ownedWeapons, saveData.weaponUpgrades, saveData.bulletsCount, saveData.playerStats, saveData.credits);

            LoadGame();
            if (saveData == null)
                Debug.Log("No save data found, initializing new inventory.");
            else
            {
                playerInventory.Credits = saveData.credits;
                //saveData.ownedWeapons.Clear();
                //foreach (var w in data.ownedWeapons) saveData.ownedWeapons.Add(w);
                //foreach (var kv in data.weaponUpgrades) saveData.weaponUpgrades[kv.Key] = kv.Value;
                //foreach (var kv in data.bulletsCount) saveData.bulletsCount[kv.Key] = kv.Value;
                //foreach (var kv in data.playerStats) saveData.playerStats[kv.Key] = kv.Value;
            }
        }
        else
        {
            saveManager = new InventorySaveManager(
                saveData.ownedWeapons,
                saveData.weaponUpgrades,
                saveData.bulletsCount,
                saveData.playerStats,
                saveData.credits
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
        saveManager = new InventorySaveManager(saveData.ownedWeapons, saveData.weaponUpgrades, saveData.bulletsCount, saveData.playerStats, saveData.credits);
        saveManager.Save();
    }
    public void LoadGame()
    {
        saveData = saveManager.Load();
    }
}
