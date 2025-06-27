using UnityEngine;

public class FakeBuyFromShop : MonoBehaviour
{
    [SerializeField] public WeaponShowCase WeaponShowCase;
    [SerializeField] Vector2 positions;
    [SerializeField] Vector2 positions1;
    [SerializeField] DeadeyeSkill deadeyeSkill;
    public void getmoney()
    {
        SaveManager.Singleton.GetComponent<PlayerInventoryHolder>().Inventory.Credits += 500;
    }
    public void Fake()
    {
        WeaponShowCase.ShowPreviousWeapon();
        WeaponShowCase.OnBuyButtonClicked();

    }
    public void fakedeadeye()
    {
        deadeyeSkill.AddTapPosition(positions);
        deadeyeSkill.AddTapPosition(positions1);
    }
}
