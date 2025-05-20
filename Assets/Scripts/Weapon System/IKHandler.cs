using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKHandler : MonoBehaviour
{
    [Header("References")]
    public Rig rig; // Reference to your specific rig (e.g. right hand IK)

    [Header("IK Weights")]
    public float shootIKWeight = 1f;
    public float normalIKWeight = 0f;
    public float ikBlendSpeed = 5f;

    private float currentIKWeight;
    public bool isShooting;

    private void Start()
    {
        currentIKWeight = rig.weight; // initialize
    }

    private void Update()
    {
        if (rig == null)
        {
            Debug.LogWarning("Rig is not assigned!");
            return;
        }


        float targetWeight = isShooting ? shootIKWeight : normalIKWeight;
        Debug.Log($"isShooting: {isShooting}, targetWeight: {targetWeight}, currentIKWeight: {currentIKWeight}");

        // Lerp towards target weight
        currentIKWeight = Mathf.Lerp(currentIKWeight, targetWeight, Time.deltaTime * ikBlendSpeed);

        // Snap to exact value if very close to avoid float precision issues
        if (Mathf.Abs(currentIKWeight - targetWeight) < 0.0001f)
        {
            currentIKWeight = targetWeight;
        }

        // Clamp to ensure it remains in [0, 1]
        currentIKWeight = Mathf.Clamp01(currentIKWeight);

        rig.weight = currentIKWeight;
    }


    public void TriggerShootIK()
    {
        Debug.Log("TriggerShootIK called"); // Add this
        isShooting = true;
        CancelInvoke(nameof(ResetShootIK));
        Invoke(nameof(ResetShootIK), 0.8f);
    }


    private void ResetShootIK()
    {
        isShooting = false;
    }
}
