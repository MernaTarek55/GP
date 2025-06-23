using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Presentation : MonoBehaviour
{
    [SerializeField] private Transform[] cameraPoints;
    [SerializeField] private int pointsIndex = 0;
    [SerializeField] private float duration = 2f;

    [SerializeField] private Transform camera;

    private void Start()
    {
        camera.position = cameraPoints[pointsIndex].position;   
    }


    private void Update()
    {
        Controllers();
    }

    private void Controllers()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveToNextSlide();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveToPreviousSlide();
        }
    }

    private void MoveToNextSlide()
    {
        StartLerp(cameraPoints[++pointsIndex]);
    }

    private void MoveToPreviousSlide()
    {
        StartLerp(cameraPoints[--pointsIndex]);
    }

    public void StartLerp(Transform destination)
    {
        StartCoroutine(LerpToPosition(destination.position, duration));
    }

    private IEnumerator LerpToPosition(Vector3 end, float time)
    {
        Vector3 start = camera.position;
        float elapsed = 0f;

        while (elapsed < time)
        {
            camera.position = Vector3.Lerp(start, end, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        camera.position = end;
    }
}
