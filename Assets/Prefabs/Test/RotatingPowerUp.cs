using UnityEngine;

public class RotatingPowerUp : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Header("Hover Settings")]
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private float hoverSpeed = 2f;

    [Header("PowerUp Type")]
    public PowerUpType powerUpType;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);

        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (powerUpType)
                {
                    case PowerUpType.Invisibility:
                        player.ActivateInvisibility();
                        break;
                    case PowerUpType.Pistol:
                        player.ActivatePistol();
                        break;
                    case PowerUpType.DeadEye:
                        player.ActivateDeadEye();
                        break;
                   
                }
                Destroy(gameObject);
            }
        }
    }
}

public enum PowerUpType
{
    Invisibility,
    Pistol,
    DeadEye
}
