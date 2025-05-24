using UnityEngine;

public class LavaProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float groundLevelPosition = -0.2f;

    [Header("References")]
    public GameObject LavaRobot;
    private Vector3 initialA;
    private Vector3 initialControl;
    private Vector3 initialB;

    private float _sampleTime;
    private QuadraticCurve curve;

    private void OnEnable()
    {
        if (LavaRobot == null)
        {
            Debug.LogError("LavaRobot reference is missing!");
            ReturnToPool();
            return;
        }

        curve = LavaRobot.GetComponentInChildren<QuadraticCurve>();
        if (curve == null || curve.A == null || curve.Control == null || curve.B == null)
        {
            Debug.LogError("Missing QuadraticCurve or control points!");
            ReturnToPool();
            return;
        }

        // Cache positions at activation
        initialA = curve.A.position;
        initialControl = curve.Control.position;
        //change if u want to change target
        initialB = curve.B.position /*new Vector3(curve.B.position.x, groundLevelPosition, curve.B.position.z)*/;
        // Initialize movement
        _sampleTime = 0f;
        transform.position = initialA;
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        _sampleTime += Time.deltaTime * speed;
        _sampleTime = Mathf.Clamp01(_sampleTime);

        Vector3 position = Evaluate(_sampleTime);
        transform.position = position;

        // Calculate direction with adaptive look-ahead
        float lookAhead = Mathf.Max(0.001f, Mathf.Min(0.1f, (1f - _sampleTime) * 0.5f));
        Vector3 nextPosition = Evaluate(_sampleTime + lookAhead);



        Debug.DrawLine(initialA, initialControl, Color.red); // A to Control point
        Debug.DrawLine(initialControl, initialB, Color.red); // Control to B point

        Vector3 direction = nextPosition - position;
        if (direction.sqrMagnitude > 0.0001f) // More precise zero check
        {
            transform.forward = direction.normalized;
        }
       
        if (_sampleTime >= 1f)
        {
            ReturnToPool();
        }
    }

    private Vector3 Evaluate(float t)
    {
        t = Mathf.Clamp01(t);
        Vector3 ac = Vector3.Lerp(initialA, initialControl, t);
        Vector3 cb = Vector3.Lerp(initialControl, initialB, t);
        return Vector3.Lerp(ac, cb, t);
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        LavaProjectilePool.Instance?.ReturnToPool(gameObject);
    }
}