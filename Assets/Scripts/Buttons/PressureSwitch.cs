using UnityEngine;
public class PressureSwitch : MonoBehaviour
{
    [SerializeField] private Door currentDoor;
    [SerializeField] private Animator animator;
    public bool backtoup = true;
    private void OnTriggerStay(Collider other)
    {
        currentDoor.AddPressureSwitch(this);
        animator.SetBool("Down", true);
    }
    public void SetButtonDown()
    {
        animator.SetBool("Down", true);
    }
    private void OnTriggerExit(Collider other)
    {
        currentDoor.RemovePressureSwitch(this);
        animator.SetBool("Down", !backtoup);
    }
}