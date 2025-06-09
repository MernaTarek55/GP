using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private PlayerInventoryHolder inventoryHolder;

    private List<WeaponType> ownedWeapons;
    private int currentWeaponIndex = 0; 
    private void Start()
    {
        if (inventoryHolder == null)
        {
            inventoryHolder = FindObjectOfType<PlayerInventoryHolder>();
        }

        ownedWeapons = inventoryHolder.Inventory.inventorySaveData.ownedWeapons;
        FilterOwnedWeapons();
        ActivateWeapon(currentWeaponIndex);
    }



    private void FilterOwnedWeapons()
    {
        // Deactivate weapons player doesn't own
        for (int i = 0; i < weapons.Length; i++)
        {
            var weapon = weapons[i].GetComponent<Weapon>();
            if (weapon != null && !ownedWeapons.Contains(weapon.WeaponType))
            {
                weapons[i].SetActive(false);
            }
        }
    }

    public void SwitchWeapons()
    {
        do
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        } while (!IsWeaponOwned(currentWeaponIndex));

        ActivateWeapon(currentWeaponIndex);
    }

    private bool IsWeaponOwned(int index)
    {
        var weapon = weapons[index].GetComponent<Weapon>();
        return weapon != null && ownedWeapons.Contains(weapon.WeaponType);
    }

    private void ActivateWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                bool shouldActivate = i == index && IsWeaponOwned(i);
                weapons[i].SetActive(shouldActivate);

                if (shouldActivate)
                {
                    var weapon = weapons[i].GetComponent<Weapon>();
                    //weapon.ApplyUpgrades(inventoryHolder.Inventory);
                }
            }
        }
    }

    public GameObject GetCurrentWeapon()
    {
        if (weapons == null || weapons.Length == 0)
        {
            Debug.LogWarning("Weapons array is null or empty.");
            return null;
        }

        if (currentWeaponIndex < 0 || currentWeaponIndex >= weapons.Length)
        {
            Debug.LogWarning("Current weapon index is out of range.");
            return null;
        }

        if (weapons[currentWeaponIndex] == null)
        {
            Debug.LogWarning("Current weapon is null.");
            return null;
        }

        return weapons[currentWeaponIndex];
    }


    public void OnWeaponPurchased(WeaponType weaponType)
    {
        // Refresh owned weapons list
        ownedWeapons = inventoryHolder.Inventory.inventorySaveData.ownedWeapons;

        // If this was the first weapon purchased, activate it
        if (ownedWeapons.Count == 1)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                var weapon = weapons[i].GetComponent<Weapon>();
                if (weapon != null && weapon.WeaponType == weaponType)
                {
                    currentWeaponIndex = i;
                    ActivateWeapon(currentWeaponIndex);
                    break;
                }
            }
        }
    }
}