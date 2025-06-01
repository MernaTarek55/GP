using UnityEngine;


public class LaserControl : MonoBehaviour
{
    private void OnDrawGizmos() { Debug.DrawLine(transform.position, transform.position + transform.forward * 50); }


}