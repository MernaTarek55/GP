using UnityEngine;

public class LavaProjectile : MonoBehaviour
{
    public GameObject LavaRobot;
    public float speed;

    private QuadraticCurve curve;

    private float sampleTime;
    private Vector3 initialA;
    private Vector3 initialControl;
    private Vector3 initialB;
    public float groundLevelPosition = -0.2f;

    //private void Awake()
    //{
    //}
    private void Start()
    {
        curve = LavaRobot.GetComponent<QuadraticCurve>();
        sampleTime = 0f;
        initialA = curve.A.position;
        initialControl = curve.Control.position;
        initialB = new Vector3(curve.B.position.x, groundLevelPosition, curve.B.position.z);
    }
   

    void Update()
    {
        if (gameObject.activeSelf)
        {
            sampleTime += Time.deltaTime * speed;

            Vector3 position = Evaluate(sampleTime);
            transform.position = position;

            Vector3 nextPosition = Evaluate(sampleTime + 0.001f);
            transform.forward = nextPosition - position;

            if (sampleTime >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
    private Vector3 Evaluate(float t)
    {

        Vector3 ac = Vector3.Lerp(initialA, initialControl, t);
        Vector3 cb = Vector3.Lerp(initialControl, initialB, t);
        return Vector3.Lerp(ac, cb, t);
    }
}