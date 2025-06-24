using UnityEngine;

public class FallingPlat : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private float resetDelay = 2f; // Time after animation to reset
    private bool m_Done = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !m_Done)
        {
            Debug.Log("Triggered falling platform.");
            m_Done = true;
            m_Animator.SetTrigger("StartFalling");
        }
    }
}
