using System.Linq;
using UnityEngine;

public class PlayerInventoryHolder : MonoBehaviour
{
    public static PlayerInventoryHolder instance;
    public PlayerInventory Inventory { get; private set; }

    //PlayerInventoryHolder 
    private void Awake()
    {
        instance = this;
        Inventory = new PlayerInventory();
    }


    private void Start()
    {
        //instance = this;

        WeaponData[] allWeaponData = Resources.LoadAll<WeaponData>("WeaponData");
        Inventory.InitializeWeaponUpgrades(allWeaponData.ToList());
        Inventory.InitializePlayerStats();
        // Initialize skill defaults
        Inventory.SetPlayerStat(PlayerSkillsStats.InvisibilityDuration, 5f); // 5 seconds default
        Inventory.SetPlayerStat(PlayerSkillsStats.InvisibilityCoolDown, 10f); // 10 seconds default
        //Inventory.AddWeapon(WeaponType.Gun);
        Inventory.PrintWeaponUpgrades();
    }

    //for testing purposes
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z pressed, adding 100 credits");
            Inventory.Credits += 100;
        }
    }
}
