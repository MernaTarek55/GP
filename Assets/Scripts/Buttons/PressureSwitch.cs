using UnityEngine;

public class PressureSwitch : MonoBehaviour
{
    public enum ButtonType
    {
        PressOnce,
        Hold
    }

    [SerializeField] private Door currentDoor;
    [SerializeField] private Animator animator;
    [SerializeField] private ButtonType buttonType = ButtonType.Hold;

    public bool backtoup = true;
    private bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (buttonType == ButtonType.PressOnce && !isPressed)
        {
            currentDoor.AddPressureSwitch(this);
            animator.SetBool("Down", true);
            isPressed = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (buttonType == ButtonType.Hold)
        {
            currentDoor.AddPressureSwitch(this);
            animator.SetBool("Down", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (buttonType == ButtonType.Hold)
        {
            currentDoor.RemovePressureSwitch(this);
            animator.SetBool("Down", false);
        }
    }

    public void SetButtonDown()
    {
        animator.SetBool("Down", true);
    }
}
