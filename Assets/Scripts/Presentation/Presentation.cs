using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Presentation : MonoBehaviour
{
    [SerializeField] private Transform PresentationPoints;
    private Transform[] cameraPoints;
    [SerializeField] private int pointsIndex = 0;
    [SerializeField] private float duration = 2f;

    [SerializeField] private Transform camera;

    private void Start()
    {
        cameraPoints = new Transform[PresentationPoints.childCount];
        for (int i = 0; i < PresentationPoints.childCount; i++)
        {
            cameraPoints[i] = PresentationPoints.GetChild(i);
            cameraPoints[i].gameObject.SetActive(false);
        }
        camera.position = cameraPoints[pointsIndex].position;
        foreach (var point in cameraPoints)
        {
            point.gameObject.SetActive(false);
        }
        cameraPoints[pointsIndex].gameObject.SetActive(true);
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
        if(pointsIndex >= cameraPoints.Length - 1) return;
        cameraPoints[pointsIndex].gameObject.SetActive(false);
        cameraPoints[++pointsIndex].gameObject.SetActive(true);
        StartLerp(cameraPoints[pointsIndex]);
    }

    private void MoveToPreviousSlide()
    {
        if (pointsIndex <= 0) return;
        cameraPoints[pointsIndex].gameObject.SetActive(false);
        cameraPoints[--pointsIndex].gameObject.SetActive(true);
        StartLerp(cameraPoints[pointsIndex]);
    }

    public void StartLerp(Transform destination)
    {
        camera.rotation = destination.rotation;
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
