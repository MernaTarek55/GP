using UnityEngine;

public class PressureSwitch : MonoBehaviour
{
    public enum ButtonType
    {
        PressOnce,
        Hold
    }

    public enum ButtonBehaviourTarget
    {
        Door ,
        MovingPlatform
    }

    [SerializeField] private Animator animator;
    [SerializeField] private ButtonType buttonType = ButtonType.Hold;
    [SerializeField] private ButtonBehaviourTarget behaviourTarget = ButtonBehaviourTarget.Door;

    public bool backtoup = true;
    private bool isPressed = false;

    [Header("Door Settings")]
    [SerializeField] private Door currentDoor;

    [Header("Optional Moving Platform Button Activate")]
    //[SerializeField] private GameObject movingPlatform;
    [SerializeField]private OptimizedMovingPlatform movingPlatformScript;
    
    private void OnTriggerEnter(Collider other)
    {
        if (buttonType == ButtonType.PressOnce && !isPressed)
        {
            //currentDoor.AddPressureSwitch(this);
            animator.SetBool("Down", true);
            isPressed = true;
            ActivateTarget();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (buttonType == ButtonType.Hold)
        {
            //currentDoor.AddPressureSwitch(this);
            animator.SetBool("Down", true);
            ActivateTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (buttonType == ButtonType.Hold)
        {
            //currentDoor.RemovePressureSwitch(this);
            animator.SetBool("Down", false);
            DeactivateTarget();
        }
    }

    public void SetButtonDown()
    {
        animator.SetBool("Down", true);
    }

    private void ActivateTarget()
    {
        switch (behaviourTarget)
        {
            case ButtonBehaviourTarget.Door:
                currentDoor?.AddPressureSwitch(this);
                break;
            case ButtonBehaviourTarget.MovingPlatform:
                if (movingPlatformScript != null)
                    movingPlatformScript.enabled = true;
                break;
        }
    }

    private void DeactivateTarget()
    {
        switch (behaviourTarget)
        {
            case ButtonBehaviourTarget.Door:
                currentDoor?.RemovePressureSwitch(this);
                break;
            //case ButtonBehaviourTarget.MovingPlatform:
            //    if (movingPlatformScript != null)
            //        movingPlatformScript.enabled = false;
            //    break;
        }
    }
}
