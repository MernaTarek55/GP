using UnityEngine;

public class PlayerInventoryHolder : MonoBehaviour
{
    public PlayerInventory Inventory { get; private set; } = new PlayerInventory();
}
