using UnityEngine;
using System.Collections;

public class LaserControl : MonoBehaviour
{
    public bool isShowingLaser = false;
    private Renderer thisRenderer;

    void Start()
    {
        thisRenderer = GetComponent<Renderer>();
    }

    public void ShowLaser()
    {
        if (isShowingLaser) return;
        StartCoroutine(ShowLaserCoroutine());
    }

    private IEnumerator ShowLaserCoroutine()
    {
        isShowingLaser = true;
        thisRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        ResetLaser();
        isShowingLaser = false;
    }

    private void ResetLaser()
    {
        // Add your laser reset logic here
        thisRenderer.enabled = false;
    }
}