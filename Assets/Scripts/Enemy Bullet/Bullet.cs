using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime =10f;
    float damage = 10;
    private void OnEnable()
    {
        Invoke(nameof(Disable), lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
    public void SetDamage(float Damage)
    {
        damage = Damage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponentInChildren<HealthComponent>().TakeDamage(damage);

        if (other.gameObject.CompareTag("Enemy"))
            other.gameObject.GetComponentInChildren<HealthComponent>().TakeDamage(damage);
        Disable();
    }
}
