using System.Buffers;
using UnityEngine;

public class MemoryPick : PickupBase
{
    public override void Pickup(GameObject player)
    {
        if (MemoryManager.Instance != null)
        {
            if (MemoryManager.Instance.PickingUpMemory(this.gameObject))
            {
                Debug.Log($"Correct memory picked: {name}");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"Wrong memory: {name}. Pick the right one.");
            }
        }
    }
}
