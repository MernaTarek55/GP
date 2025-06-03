using System.Linq;
using UnityEngine;

public class PlayerInventoryHolder : MonoBehaviour
{
    public static PlayerInventoryHolder instance;
    public PlayerInventory Inventory { get; private set; } = new PlayerInventory();

    //PlayerInventoryHolder 


    private void Awake()
    {
        instance = this;

        WeaponData[] allWeaponData = Resources.LoadAll<WeaponData>("WeaponData");
        Inventory.InitializeWeaponUpgrades(allWeaponData.ToList());
        // Initialize skill defaults
        Inventory.SetPlayerStat(PlayerSkillsStats.InvesabilityDuration, 5f); // 5 seconds default
        Inventory.SetPlayerStat(PlayerSkillsStats.InvesabilityCoolDown, 10f); // 10 seconds default
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
