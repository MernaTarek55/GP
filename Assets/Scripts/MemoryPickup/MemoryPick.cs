using System.Buffers;
using UnityEngine;

public class MemoryPick : PickupBase
{
    //[SerializeField] string memoryName;
    //public override void Pickup(GameObject player)
    //{
    //    if (MemoryManager.Instance != null)
    //    {
    //        if (MemoryManager.Instance.PickingUpMemory(memoryName))
    //        {
    //            Debug.Log($"Correct memory picked: {memoryName}");
    //            Destroy(gameObject);
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"Wrong memory: {memoryName}. Pick the right one.");
    //        }
    //    }
    //}

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
