using DG.Tweening;
using UnityEngine;
public class SlideDoor : MonoBehaviour
{
    //[SerializeField] private float moveAmount = 2;
    [SerializeField] private Vector3 rotationAmount = new Vector3(0,90,0);
    [SerializeField] private float duration = 1;
    //[SerializeField] private Vector3 endPos;
    private Vector3 startRotation;
    //private Vector3 startPos;
    private void Awake()
    {
        startRotation = transform.localEulerAngles;
    }

    [ContextMenu("Open")]
    public void Open()
    {
        //transform.DOLocalMove(transform.localPosition + endPos, duration);
        transform.DOLocalRotate(startRotation + rotationAmount, duration);
    }

    [ContextMenu("Close")]
    public void Close()
    {
        //transform.DOLocalMove(startPos, duration);
        transform.DOLocalRotate(startRotation, duration);

    }
}