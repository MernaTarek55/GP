using System.Linq;
using UnityEngine;

public class PlayerInventoryHolder : MonoBehaviour
{
    public PlayerInventory Inventory { get; private set; } = new PlayerInventory();

    //PlayerInventoryHolder 


    private void Awake()
    {
        WeaponData[] allWeaponData = Resources.LoadAll<WeaponData>("WeaponData");
        Inventory.InitializeWeaponUpgrades(allWeaponData.ToList());
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
