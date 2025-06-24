using DG.Tweening;
using UnityEngine;
public class SlideDoor : MonoBehaviour
{
    [SerializeField] private float duration = 1;
    [SerializeField] private Vector3 endPos;
    private Vector3 closedRotation;
    private void Awake()
    {
        closedRotation = transform.localEulerAngles;
    }

    [ContextMenu("Open")]
    public void Open()
    {
        Vector3 targetRotation = new Vector3(closedRotation.x, closedRotation.y, closedRotation.z);
        transform.DOLocalRotate(targetRotation + endPos, duration);
    }

    [ContextMenu("Close")]
    public void Close()
    {
        transform.DOLocalRotate(closedRotation, duration);

    }
}