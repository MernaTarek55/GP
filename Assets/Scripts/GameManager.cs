using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //just temp calss



    public PlayerInventory playerInventory = new();

    void Start()
    {
        WeaponData[] allWeaponData = Resources.LoadAll<WeaponData>("WeaponData");
        playerInventory.InitializeWeaponUpgrades(allWeaponData.ToList());
    }
}
