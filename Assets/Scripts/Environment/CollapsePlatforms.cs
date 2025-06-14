using UnityEngine;
using DG.Tweening;

public class CollapsePlatforms : MonoBehaviour
{
    private Rigidbody rb;
    private bool isCollapsing = false;
    [SerializeField] float endYPosition = 10f;
    [SerializeField] float collapsePositionduration = 2f;
    [SerializeField] float collapseRotationDuration = 1.5f;
    [SerializeField] float collapseRotationAngle = 90f;
    [SerializeField] float destroyDuration = 1.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isCollapsing && collision.gameObject.CompareTag("Player"))
        {
            isCollapsing = true;
            CollapseAnimation();
        }
    }

    private void CollapseAnimation()
    {
        // Optional: make Rigidbody kinematic to allow DOTween to control transform
        rb.isKinematic = true;

        Sequence collapseSequence = DOTween.Sequence();

        collapseSequence.Append(transform.DORotate(new Vector3(0, 0, collapseRotationAngle), collapseRotationDuration, RotateMode.Fast))
                        .Join(transform.DOMoveY(transform.position.y - endYPosition, collapsePositionduration))
                        .OnComplete(() => Destroy(gameObject, destroyDuration));
    }
}
