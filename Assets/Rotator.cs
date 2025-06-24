using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateObject();
    }
    private void RotateObject()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
