using UnityEngine;

public class MagneticField : MonoBehaviour
{
    [SerializeField] private float forceStrength = 10f;
    [SerializeField] private float magnetRange = 5f;
    [SerializeField] private LayerMask affectedLayers;

    private Collider[] results = new Collider[20]; 

    private void FixedUpdate()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, magnetRange, results, affectedLayers);

        for (int i = 0; i < count; i++)
        {
            Rigidbody rb = results[i].attachedRigidbody;
            if (rb == null || rb.isKinematic) continue;

            Vector3 direction = transform.position - rb.position;
            float distance = direction.magnitude;
            if (distance < 0.01f) continue;

            direction /= distance; 

            float falloff = 1f - (distance / magnetRange);
            rb.AddForce(direction * forceStrength * falloff, ForceMode.Force);
        }
    }
}
