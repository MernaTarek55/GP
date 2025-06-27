using UnityEngine;

public class FakeBuyFromShop : MonoBehaviour
{
    [SerializeField] public WeaponShowCase WeaponShowCase;
    public void getmoney()
    {
        SaveManager.Singleton.GetComponent<PlayerInventoryHolder>().Inventory.Credits += 500;
    }
    public void Fake()
    {
        WeaponShowCase.ShowPreviousWeapon();
        WeaponShowCase.OnBuyButtonClicked();
    }

}
