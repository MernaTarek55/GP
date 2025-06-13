using UnityEngine;

public class HybridLaser : MonoBehaviour
{
    [SerializeField] private ParticleSystem laserParticles;
    [SerializeField] private ParticleSystem impactParticles;
    [SerializeField] private float range = 50f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ShootContinuous();
        }
    }

    void ShootContinuous()
    {
        var emitParams = new ParticleSystem.EmitParams();
        Vector3 endPoint;
        if (Physics.Raycast(transform.position, transform.forward,
            out RaycastHit hit, range))
        {
            endPoint = hit.point;
            impactParticles.transform.position = endPoint;
            impactParticles.Emit(1);
        }
        else
        {
            endPoint = transform.position + transform.forward * range;
        }

        // Create laser beam particles
        emitParams.startLifetime = Vector3.Distance(transform.position, endPoint) / 50f;
        emitParams.position = transform.position;
        emitParams.velocity = transform.forward * 50f;
        laserParticles.Emit(emitParams, 1);
    }
}