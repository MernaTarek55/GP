using UnityEngine;

public class QuadraticCurve : MonoBehaviour
{
    [Header("Control Points")]
    public Transform A;
    public Transform B;
    public Transform Control;

    public Vector3 evaluate(float t)
    {
        if (A == null || B == null || Control == null)
        {
            Debug.LogError("Missing curve control points!");
            return Vector3.zero;
        }

        t = Mathf.Clamp01(t);
        Vector3 ac = Vector3.Lerp(A.position, Control.position, t);
        Vector3 cb = Vector3.Lerp(Control.position, B.position, t);
        return Vector3.Lerp(ac, cb, t);
    }

    private void OnDrawGizmos()
    {
        if (A == null || B == null || Control == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        for (int i = 0; i <= 20; i++)
        {
            float t = i / 20f;
            //Gizmos.DrawSphere(evaluate(t), 0.05f);
        }

        // Draw control lines
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(A.position, Control.position);
        //Gizmos.DrawLine(Control.position, B.position);
    }
}