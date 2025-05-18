using UnityEngine;

public abstract class PickupBase : MonoBehaviour, IPickupInterface
{
    public abstract void Pickup(GameObject player);
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other.gameObject);
            //Destroy(gameObject);
        }
    }
}
