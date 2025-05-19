using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected float damage;

    public virtual void SetDamage(float damage)
    {
        this.damage = damage;
    }

    protected virtual void OnHit(Collider other)
    {
        // Override this in derived classes for specific collision behavior
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        OnHit(other);
    }
}
