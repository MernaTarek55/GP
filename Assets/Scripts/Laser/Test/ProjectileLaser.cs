using UnityEngine;
public class ProjectileLaser : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private LineRenderer trailRenderer;

    private Vector3 startPosition;
    private bool hasHit = false;

    void Start()
    {
        startPosition = transform.position;
        Destroy(gameObject, 2f); // Auto-destroy after 2 seconds
    }

    void Update()
    {
        if (!hasHit)
        {
            float moveDistance = speed * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);

            // Update trail
            if (trailRenderer != null)
            {
                trailRenderer.SetPosition(0, startPosition);
                trailRenderer.SetPosition(1, transform.position);
            }
        }
    }

    void CheckCollisions(float distance)
    {
        if (Physics.Raycast(transform.position, transform.forward,
            out RaycastHit hit, distance))
        {
            OnHit(hit);
        }
    }

    void OnHit(RaycastHit hit)
    {
        hasHit = true;
        Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(gameObject);
    }
}