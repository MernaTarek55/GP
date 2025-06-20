using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    [SerializeField]Door door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.OpenDoor();
        }
    }
}
