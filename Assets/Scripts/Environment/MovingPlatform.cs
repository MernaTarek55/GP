using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum Direction { Horizontal, Vertical, Custom }

    [Header("Movement Settings")]
    public Direction moveDirection = Direction.Horizontal;
    public Vector3 customDirection = Vector3.right;
    public float distance = 5f;
    public float speed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingToTarget = true;
    //TODO : if the moving platform aint too much in the scene reconsider this
    private bool isVisible = false;

    void Start()
    {
        startPos = transform.position;

        switch (moveDirection)
        {
            case Direction.Horizontal:
                targetPos = startPos + Vector3.right * distance;
                break;
            case Direction.Vertical:
                targetPos = startPos + Vector3.up * distance;
                break;
            case Direction.Custom:
                targetPos = startPos + customDirection.normalized * distance;
                break;
        }
    }

    void Update()
    {
        if (!isVisible) return;

        Vector3 destination = movingToTarget ? targetPos : startPos;
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.01f)
            movingToTarget = !movingToTarget;
    }

    void OnBecameVisible() => isVisible = true;

    void OnBecameInvisible() => isVisible = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.transform.SetParent(transform);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        //TODO : what is player real parent?
            collision.transform.SetParent(null);
    }

}
