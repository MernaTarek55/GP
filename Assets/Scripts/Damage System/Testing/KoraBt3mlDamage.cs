using UnityEngine;

public class KoraBt3mlDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponent<HealthComponent>().TakeDamage(10);

        if (other.gameObject.CompareTag("Destructible"))
            other.gameObject.GetComponent<DestructibleObject>().health.TakeDamage(100);
    }
}
