using UnityEngine;

public class LavaTraceManager : MonoBehaviour
{
    private Transform camTransform;
    private void Awake()
    {
        camTransform = Camera.main.transform;

    }

    private void FixedUpdate()
    {
        if (camTransform != null)
        {
            transform.LookAt(camTransform);
            transform.RotateAround(transform.position, transform.right, 90);
        }
    }
}
