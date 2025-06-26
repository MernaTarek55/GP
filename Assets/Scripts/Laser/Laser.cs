using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private LaserRendererSettings laserRendererSettings;
    [SerializeField] private GameObject inputGO;
    [SerializeField][Range(1, 3)] private int maxBounces = 3;
    [SerializeField] private float lifetime = 0.05f; // Only used when IsNotTurret is true

    [Header("Debug")]
    [SerializeField] private bool drawDebugRays = false;
    [SerializeField] private bool IsNotTurret = false;
    [SerializeField] private PoolType poolType = PoolType.Laser;

    [Header("Tracking")]
    [SerializeField] private bool trackPlayer = false;
    [SerializeField] private float trackingSpeed = 10f;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private bool hasDoneDamage; // Nano: bool to check if the same laser causes damage twice
    private bool activated = false;
    [SerializeField] private LineRenderer lineRenderer;
    private const float farDistance = 1000f;
    private List<Vector3> bouncePositions;
    private LaserSensor prevStruckSensor = null;
    private IInput input;
    private float currentLifetime;

    private float laserDamage = 20; // Nano: to set the damage of the laser
    private Vector3 currentLaserDirection;


    void Awake()
    {
        InitializeLineRenderer();
        InitializeInputSystem();
        activated = inputGO == null; // Activate if no input is assigned

        hasDoneDamage = false;

        if (IsNotTurret)
        {
            currentLifetime = lifetime;
        }
        //lineRenderer.GetComponent<LineRenderer>();
        currentLaserDirection = transform.forward;

    }

    private void UpdateLaserDirection()
    {
        if (!trackPlayer || !hasTarget) return;

        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        if (IsNotTurret)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, trackingSpeed * Time.deltaTime);

        }
        else
        {
            // Lerp the actual beam direction
            //enemyGO.transform.LookAt(playerGO.transform);

            currentLaserDirection = Vector3.Lerp(currentLaserDirection, targetDirection, trackingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, trackingSpeed * Time.deltaTime);
            this.transform.parent.rotation = Quaternion.Lerp(this.transform.parent.rotation, targetRotation, trackingSpeed * Time.deltaTime);
        }
    }

    private void InitializeLineRenderer()
    {
        //lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.enabled = IsNotTurret;
        lineRenderer.positionCount = 2;
        //lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        //lineRenderer.SetPosition(1, new Vector3(0, 0, 0.1f));
        if (laserRendererSettings == null)
        {
            Debug.LogError("LaserRendererSettings is not assigned!", this);
            enabled = false;
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

    void OnEnable()
    {
        if (IsNotTurret)
        {
            currentLifetime = lifetime;
            activated = true;
            ClearLaser(); // Reset laser state when re-enabled
        }
    }

    //void FixedUpdate()
    void Update()
    {
        if (IsNotTurret)
        {
            UpdateLifetime();
        }

        if (!activated)
        {
            ClearLaser();
            return;
        }
        UpdateLaserDirection();
        UpdateLaserBeam();
    }

    private void UpdateLifetime()
    {
        currentLifetime -= Time.fixedDeltaTime;
        if (currentLifetime <= 0f)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.ReturnToPool(poolType, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Updated methods for Vector3 targeting
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
        trackPlayer = true;
    }

    // Legacy method for Transform - converts to Vector3
    public void SetTarget(Transform target)
    {
        if (target != null)
        {
            SetTargetPosition(target.position);
        }
        else
        {
            ClearTarget();
        }
    }

    public void ClearTarget()
    {
        hasTarget = false;
        trackPlayer = false;
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

        hasDoneDamage = false;
    }

    private void UpdateLaserBeam()
    {
        if (lineRenderer == null) return;

        bouncePositions = new List<Vector3>
        {
            transform.position + transform.forward * 0.2501f
        };

        if (IsNotTurret)
        {
            CastBeam(bouncePositions[0], transform.forward);
        }
        else
        {
            CastBeam(bouncePositions[0], currentLaserDirection);
        }

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

            // For bullet-like behavior, check if we hit something that should stop the laser
            IDamageable damagable = hitInfo.collider.GetComponent<IDamageable>();

            if (damagable != null)
            {
                bool isHittingSelf = hitInfo.collider.gameObject.tag == gameObject.tag;

                if (!IsNotTurret && !isHittingSelf)
                {
                    // It's NOT a turret laser, damage any target except itself
                    damagable.TakeDamage(laserDamage);
                    Debug.Log("Lase");
                    currentLifetime = 0;

                }
                else if (IsNotTurret && !hasDoneDamage && !isHittingSelf)
                {
                    // It's a turret laser, damage any target except itself (and only once)
                    damagable.TakeDamage(laserDamage);
                    currentLifetime = 0;

                    hasDoneDamage = true;
                }
            }

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

    // Call this when spawning the laser to initialize it
    public void InitializeLaser(Vector3 position, Quaternion rotation, bool isFromPool = true)
    {
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
        currentLifetime = lifetime;

        // Reset any previous state
        ClearLaser();
    }

    // Nano: call this function to set damage
    public void SetLaserDamage(float damage)
    {
        laserDamage = damage;
    }
}