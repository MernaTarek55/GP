using UnityEngine;

public abstract class PickupBase : MonoBehaviour, IPickupInterface
{
    public abstract void Pickup(GameObject player);
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("trigger enter ");
            Pickup(other.gameObject);
            //Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("collision enter ");
            Pickup(collision.gameObject);
            //Destroy(gameObject);
        }
    }

}
