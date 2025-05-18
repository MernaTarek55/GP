using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class DestructibleObject : MonoBehaviour
{
    //private HealthComponent health;
    public HealthComponent health;

    private void Awake()
    {
        health = GetComponent<HealthComponent>();
    }

    private void Update()
    {
        if (health != null && health.IsDead())
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
        Debug.Log("Object is Destroyed");
    }
}
