using UnityEngine;

public class ManageTimeLine : MonoBehaviour
{
    [SerializeField] private GameObject timelinePrefab;
    Vector3 initialPosition;
    bool actevate = false;
    private void OnTriggerEnter(Collider other)
    {
        if(!actevate)
        {
            actevate = true;
        timelinePrefab.SetActive(true);
        }
    }
    public void Reset()
    {
        timelinePrefab.SetActive(false);
        
    }
}
