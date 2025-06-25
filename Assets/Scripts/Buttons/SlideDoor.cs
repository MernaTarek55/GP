using DG.Tweening;
using UnityEngine;
public class SlideDoor : MonoBehaviour
{
    [SerializeField] private float moveAmount = 2;
    [SerializeField] private float duration = 1;
    [SerializeField] private Vector3 endPos;
    private Vector3 startPos;
    private void Awake()
    {
        startPos = transform.localPosition;
    }

    [ContextMenu("Open")]
    public void Open()
    {
        transform.DOLocalMove(transform.localPosition + endPos, duration);
    }

    [ContextMenu("Close")]
    public void Close()
    {
        transform.DOLocalMove(startPos, duration);
    }
}