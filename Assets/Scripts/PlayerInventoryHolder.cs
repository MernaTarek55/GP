using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventoryHolder : MonoBehaviour
{
    public static PlayerInventoryHolder instance;
    public PlayerInventory Inventory { get; private set; }

    [SerializeField] List<WeaponData> allWeaponData;
    //PlayerInventoryHolder 
    private void Awake()
    {
        instance = this;
        Inventory = new PlayerInventory();
    }


    private void Start()
    {
        //instance = this;
        if(allWeaponData.Count < 1)
            Debug.LogWarning("No weapon data found, please assign weapon data in the inspector.");
        Inventory.InitializeWeaponUpgrades(allWeaponData);
        Inventory.InitializePlayerStats();
        // Initialize skill defaults
        Inventory.SetPlayerStat(PlayerSkillsStats.InvisibilityDuration, 20f); 
        Inventory.SetPlayerStat(PlayerSkillsStats.InvisibilityCoolDown, 10f); 
        Inventory.SetPlayerStat(PlayerSkillsStats.DeadEyeDuration, 5f); 
        Inventory.SetPlayerStat(PlayerSkillsStats.DeadEyeCoolDown, 10f); 
        //Inventory.AddWeapon(WeaponType.Gun);
        //Inventory.PrintWeaponUpgrades();
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
