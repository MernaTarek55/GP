using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private LaserRendererSettings laserRendererSettings;
    [SerializeField] private GameObject inputGO;
    [SerializeField][Range(1, 100)] private int maxBounces = 3;

    [Header("Debug")]
    [SerializeField] private bool drawDebugRays = false;

    private bool activated = false;
    private LineRenderer lineRenderer;
    private Vector3 sourcePosition;
    private const float farDistance = 1000f;
    private List<Vector3> bouncePositions;
    private LaserSensor prevStruckSensor = null;
    private IInput input;

    void Awake()
    {
        InitializeLineRenderer();
        InitializeInputSystem();
        activated = inputGO == null; // Activate if no input is assigned
    }

    private void InitializeLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        if (laserRendererSettings == null)
        {
            Debug.LogError("LaserRendererSettings is not assigned!", this);
            enabled = false; // Disable the script
            return;
        }

        laserRendererSettings.Apply(lineRenderer);
    }

    private void InitializeInputSystem()
    {
        if (inputGO == null) return;

        input = inputGO.GetComponent<IInput>();
        if (input == null)
        {
            Debug.LogWarning($"No IInput component found on {inputGO.name}", inputGO);
            return;
        }

        RegisterToInput(input);
    }

    void FixedUpdate()
    {
        if (!activated)
        {
            ClearLaser();
            return;
        }

        UpdateLaserBeam();
    }

    private void ClearLaser()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }

        if (prevStruckSensor != null)
        {
            LaserSensor.HandleLaser(this, prevStruckSensor, null);
            prevStruckSensor = null;
        }
    }

    private void UpdateLaserBeam()
    {
        if (lineRenderer == null) return;

        bouncePositions = new List<Vector3>
        {
            transform.position + transform.forward * 0.2501f
        };

        CastBeam(bouncePositions[0], transform.forward);

        if (bouncePositions.Count > 0)
        {
            lineRenderer.positionCount = bouncePositions.Count;
            lineRenderer.SetPositions(bouncePositions.ToArray());
        }
    }

    public void CastBeam(Vector3 origin, Vector3 direction)
    {
        if (bouncePositions == null || bouncePositions.Count > maxBounces)
        {
            return;
        }

        if (drawDebugRays)
        {
            Debug.DrawRay(origin, direction * 100, Color.red, 0.1f);
        }

        var ray = new Ray(origin, direction);
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, farDistance))
        {
            HandleMissedRaycast(origin, direction);
            return;
        }

        bouncePositions.Add(hitInfo.point);
        HandleRaycastHit(ray, hitInfo);
    }

    private void HandleMissedRaycast(Vector3 origin, Vector3 direction)
    {
        bouncePositions.Add(origin + direction * farDistance);
        UpdateSensorState(null);
    }

    private void HandleRaycastHit(Ray ray, RaycastHit hitInfo)
    {
        var reflectiveObject = hitInfo.collider.GetComponent<ILaserReflective>();
        if (reflectiveObject != null)
        {
            reflectiveObject.Reflect(this, ray, hitInfo);
        }
        else
        {
            var currentSensor = hitInfo.collider.GetComponent<LaserSensor>();
            UpdateSensorState(currentSensor);
        }
    }

    private void UpdateSensorState(LaserSensor currentSensor)
    {
        if (currentSensor != prevStruckSensor)
        {
            LaserSensor.HandleLaser(this, prevStruckSensor, currentSensor);
            prevStruckSensor = currentSensor;
        }
    }

    private void RegisterToInput(IInput input)
    {
        if (this.input != null)
        {
            this.input.onTriggered -= OnInputTriggered;
            this.input.onUntriggered -= OnInputUntriggered;
        }

        this.input = input;
        if (input != null)
        {
            input.onTriggered += OnInputTriggered;
            input.onUntriggered += OnInputUntriggered;
            activated = input.IsTriggered;
        }
    }

    private void OnInputTriggered(IInput source) => activated = true;
    private void OnInputUntriggered(IInput source) => activated = false;

    void OnDestroy()
    {
        if (input != null)
        {
            input.onTriggered -= OnInputTriggered;
            input.onUntriggered -= OnInputUntriggered;
        }
    }

    public void SetActive(bool active)
    {
        activated = active;
        if (!active && prevStruckSensor != null)
        {
            LaserSensor.HandleLaser(this, prevStruckSensor, null);
            prevStruckSensor = null;
        }
    }
}