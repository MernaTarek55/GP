using System.Collections;
using UnityEngine;

public class HitscanLaser : MonoBehaviour
{
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private float maxRange = 100f;
    [SerializeField] private float displayTime = 0.1f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, maxRange))
        {
            laserLine.SetPosition(0, rayOrigin);
            laserLine.SetPosition(1, hit.point);
            OnHit(hit);
        }
        else
        {
            laserLine.SetPosition(0, rayOrigin);
            laserLine.SetPosition(1, rayOrigin + rayDirection * maxRange);
        }

        StartCoroutine(ShowLaser());
    }

    IEnumerator ShowLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(displayTime);
        laserLine.enabled = false;
    }

    void OnHit(RaycastHit hit)
    {
        // Handle damage or effects
    }
}