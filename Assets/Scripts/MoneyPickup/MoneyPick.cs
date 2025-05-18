using UnityEngine;

public class MoneyPick : PickupBase
{
    public override void Pickup(GameObject player)
    {
        MoneyCount.instance.AddMoney(1);
        Destroy(gameObject);
    }
}
