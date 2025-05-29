using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    private int currentWeaponIndex = 0;
    private readonly PlayerInventory inventory;
    private void Start()
    {

        ActivateWeapon(currentWeaponIndex);

    }

   

    public void SwitchWeapons()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        ActivateWeapon(currentWeaponIndex);

    }

    private void ActivateWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(i == index);
            }
        }
    }
}
