using UnityEngine;

public class LavaDropDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HealthComponent>().TakeDamage(10);
        }
    }
}
