using System.Collections.Generic;
using UnityEngine;
public class Door : MonoBehaviour
{
    public bool IsDoorOpen = false;
    [SerializeField] private int requiredSwitchesToOpen = 1;

    [SerializeField] private SlideDoorManager slideDoorManager;

    private List<PressureSwitch> currentSwitchesOpen = new();
    [SerializeField]GameObject Switch ;
    public int CurrentSwitchesOpen => currentSwitchesOpen.Count;
    bool open = false;
    public void AddPressureSwitch(PressureSwitch currentSwitch)
    {
        if (!currentSwitchesOpen.Contains(currentSwitch))
        {
            currentSwitchesOpen.Add(currentSwitch);
        }
        TryOpen();
    }

    public void RemovePressureSwitch(PressureSwitch currentSwitch)
    {
        if (currentSwitchesOpen.Contains(currentSwitch))
        {
            currentSwitchesOpen.Remove(currentSwitch);
        }
        TryOpen();
    }
    private void TryOpen()
    {
        if (CurrentSwitchesOpen == requiredSwitchesToOpen)
        {
            foreach (var pressureSwitch in currentSwitchesOpen)
            {
                pressureSwitch.backtoup = false;
                pressureSwitch.SetButtonDown();
            }
            Switch.SetActive(true);
            open = true;
        }
        else if (CurrentSwitchesOpen < requiredSwitchesToOpen && !open)
        {
            foreach (var pressureSwitch in currentSwitchesOpen)
            {
                pressureSwitch.backtoup = true;
            }
            CloseDoor();
        }
    }


    private void CloseDoor()
    {
        if (IsDoorOpen)
        {
            slideDoorManager.CloseDoors();

        }
        IsDoorOpen = false;

    }

    public void OpenDoor()
    {
        if (!IsDoorOpen)
        {
            slideDoorManager.OpenDoors();
        }
        IsDoorOpen = true;

    }
}