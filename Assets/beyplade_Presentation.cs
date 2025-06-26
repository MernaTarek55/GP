using UnityEngine;

public class beyplade_Presentation : MonoBehaviour
{
    private Vector3 sPos;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sPos = transform.position;
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        transform.position = sPos;
        animator.SetTrigger("StartOver");
    }

}
