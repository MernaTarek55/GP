using UnityEngine;

public class ParticleLaser : MonoBehaviour
{
    private float laserDamage;

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.GetComponent<IDamageable>() != null)
        {
            IDamageable enemyDamage = other.gameObject.GetComponent<IDamageable>();
            enemyDamage.TakeDamage(laserDamage);
        }
    }

    public void SetLaserDamage(float damage)
    {
        laserDamage = damage;  
    }
}
