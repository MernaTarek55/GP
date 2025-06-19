using DG.Tweening;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float pauseDuration = 1f;
    [SerializeField] private Ease easeType = Ease.InOutSine;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 topPosition;
    private Sequence elevatorSequence;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Store the starting position
        startPosition = transform.position;
        topPosition = startPosition + Vector3.up * moveDistance;

        StartElevatorMovement();
    }

    private void StartElevatorMovement()
    {
        // Kill any existing sequence to avoid conflicts
        elevatorSequence?.Kill();

        // Create a sequence for continuous up and down movement
        elevatorSequence = DOTween.Sequence();

        // Move up
        elevatorSequence.Append(transform.DOMoveY(topPosition.y, moveDuration).SetEase(easeType));

        // Pause at the top
        elevatorSequence.AppendInterval(pauseDuration);

        // Move down
        elevatorSequence.Append(transform.DOMoveY(startPosition.y, moveDuration).SetEase(easeType));

        // Pause at the bottom
        elevatorSequence.AppendInterval(pauseDuration);

        // Loop the sequence infinitely
        elevatorSequence.SetLoops(-1, LoopType.Restart);
    }

    private void OnDestroy()
    {
        // Clean up the tween when the object is destroyed
        elevatorSequence?.Kill();
    }

    // Optional: Method to stop the elevator
    public void StopElevator()
    {
        elevatorSequence?.Kill();
    }

    // Optional: Method to restart the elevator
    public void RestartElevator()
    {
        StartElevatorMovement();
    }

    // Optional: Method to change elevator speed during runtime
    public void SetElevatorSpeed(float newDuration)
    {
        moveDuration = newDuration;
        RestartElevator();
    }
}