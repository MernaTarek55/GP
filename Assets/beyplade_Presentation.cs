using UnityEngine;

public class beyplade_Presentation : MonoBehaviour
{
    private Vector3 sPos;
    [SerializeField]private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sPos = transform.position;
    }
    private void OnEnable()
    {
        transform.position = sPos;
        animator.SetTrigger("StartOver");
    }

}
