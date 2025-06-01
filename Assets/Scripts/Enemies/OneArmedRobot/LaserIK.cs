using System;
using UnityEngine;

public class LaserIK : MonoBehaviour
{
    public Transform targetTransform;
    public Transform aimTransform;
    public Transform bone;
    void Start()
    {
        
    }

    void LateUpdate() 
    {
        Vector3 targetPosition = targetTransform.position;
        AimAtTarget(bone, targetPosition);


    }

    private void AimAtTarget(Transform bone, Vector3 targetPosition)
    {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        bone.rotation = aimTowards * bone.rotation;
    }
}
