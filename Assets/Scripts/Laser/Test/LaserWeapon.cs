using UnityEngine;

public class LaserWeapon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float fireRate = 0.2f;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);

        // For object pooling (recommended):
        // GameObject laser = LaserPool.Instance.GetLaser();
        // laser.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        // laser.SetActive(true);
    }
}