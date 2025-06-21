using UnityEngine;
public class SlideDoorManager : MonoBehaviour
{
    [SerializeField] private SlideDoor[] slideDoors;

    [ContextMenu("Open")]
    public void OpenDoors()
    {
        foreach (var slideDoor in slideDoors)
        {
            slideDoor.Open();
        }
    }
    public void CloseDoors()
    {
        foreach (var slideDoor in slideDoors)
        {
            slideDoor.Close();
        }
    }
}