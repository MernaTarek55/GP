using UnityEngine;

public class MoneyPick : PickupBase
{
    public override void Pickup(GameObject player)
    {
        player.GetComponent<PlayerInventoryHolder>().Inventory.Credits += 1; 
        //MoneyCount.instance.AddMoney(1);
        Destroy(gameObject);
    }
}
