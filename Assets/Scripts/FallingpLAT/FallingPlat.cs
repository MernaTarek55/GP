using UnityEngine;
using System.Collections;
public class FallingPlat : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float resetDelay = 2f;
    private bool m_Done = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !m_Done)
        {
            Debug.Log("Triggered falling platform.");
            m_Done = true;
            m_Animator.SetTrigger("StartFalling");
            StartCoroutine(WaitForRestPlatform());
        }
    }

    IEnumerator WaitForRestPlatform()
    {
        yield return new WaitForSeconds(resetDelay);

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        m_Done = false;
    }
    
}
