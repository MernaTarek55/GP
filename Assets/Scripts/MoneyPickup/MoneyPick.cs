using UnityEngine;

public class MoneyPick : PickupBase
{
    public override void Pickup(GameObject player)
    {
        player.GetComponent<PlayerInventoryHolder>().Inventory.Credits += 500; 
        //MoneyCount.instance.AddMoney(1);
        Destroy(gameObject);
    }
}
