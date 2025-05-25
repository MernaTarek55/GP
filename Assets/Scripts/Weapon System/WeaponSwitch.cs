using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    private int currentWeaponIndex = 0;
    PlayerInventory inventory;
    void Start()
    {

        ActivateWeapon(currentWeaponIndex);
    }

    public void SwitchWeapons()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        ActivateWeapon(currentWeaponIndex);
        
    }

    void ActivateWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                weapons[i].SetActive(i == index);
        }
    }
}
