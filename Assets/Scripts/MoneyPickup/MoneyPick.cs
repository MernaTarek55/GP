using UnityEngine;

public class MoneyPick : PickupBase
{
    public override void Pickup(GameObject player)
    {
        SaveManager.Singleton.GetComponent<PlayerInventoryHolder>().Inventory.Credits += 500; 
        //MoneyCount.instance.AddMoney(1);
        Destroy(gameObject);
    }
}
