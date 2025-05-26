using UnityEngine;

public class LavaTraceManager : MonoBehaviour
{
    Transform camTransform;
    private void Awake()
    {
        camTransform = Camera.main.transform;

    }

    void FixedUpdate()
    {
        if (camTransform != null)
        {
            transform.LookAt(camTransform);
            transform.RotateAround(transform.position, transform.right, 90);
        }
    }
}
