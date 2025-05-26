using UnityEngine;
public class EnemyBullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;

    private void OnEnable()
    {
        Invoke(nameof(Disable), lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HealthComponent>().TakeDamage(10);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<HealthComponent>().TakeDamage(10);
        }

        Disable();
    }
}
