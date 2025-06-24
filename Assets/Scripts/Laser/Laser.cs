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

    private bool hasDoneDamage;
    private bool activated = false;

    [SerializeField] private TrailRenderer trailRenderer;
    private const float farDistance = 1000f;
    private List<Vector3> bouncePositions;
    private LaserSensor prevStruckSensor = null;
    private IInput input;
    private float currentLifetime;

    private float laserDamage = 20;
    private Vector3 currentLaserDirection;

    void Awake()
    {
        InitializeTrailRenderer();
        InitializeInputSystem();
        activated = inputGO == null;
        hasDoneDamage = false;

        if (IsNotTurret)
        {
            currentLifetime = lifetime;
        }

        currentLaserDirection = transform.forward;
    }

    void OnEnable()
    {
        if (IsNotTurret)
        {
            currentLifetime = lifetime;
            activated = true;
            ResetTrail();
        }
    }

    void Update()
    {
        if (IsNotTurret)
        {
            UpdateLifetime();
        }

        if (!activated)
        {
            ResetTrail();
            return;
        }

        UpdateLaserDirection();
        UpdateLaserMovement();
    }

    private void UpdateLifetime()
    {
        currentLifetime -= Time.deltaTime;
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

    private void InitializeTrailRenderer()
    {
        if (trailRenderer == null)
        {
            trailRenderer = gameObject.GetComponent<TrailRenderer>();
        }

        if (trailRenderer == null)
        {
            trailRenderer = gameObject.AddComponent<TrailRenderer>();
        }

        trailRenderer.enabled = IsNotTurret;

        if (laserRendererSettings != null)
        {
            laserRendererSettings.Apply(trailRenderer);
        }
        else
        {
            Debug.LogError("LaserRendererSettings is not assigned!", this);
        }
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
            currentLaserDirection = Vector3.Lerp(currentLaserDirection, targetDirection, trackingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, trackingSpeed * Time.deltaTime);
        }
    }

    private void UpdateLaserMovement()
    {
        Vector3 direction = IsNotTurret ? transform.forward : currentLaserDirection;
        Vector3 origin = transform.position;

        bouncePositions = new List<Vector3>
        {
            origin + direction * 0.25f
        };

        CastBeam(origin, direction);
        transform.position += direction * Time.deltaTime * 100f; // adjust speed if needed
    }

    public void CastBeam(Vector3 origin, Vector3 direction)
    {
        if (bouncePositions == null || bouncePositions.Count > maxBounces) return;

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

            IDamageable damagable = hitInfo.collider.GetComponent<IDamageable>();
            bool isHittingSelf = hitInfo.collider.gameObject.tag == gameObject.tag;

            if (damagable != null && !isHittingSelf)
            {
                if (!IsNotTurret || (IsNotTurret && !hasDoneDamage))
                {
                    damagable.TakeDamage(laserDamage);
                    hasDoneDamage = true;
                    currentLifetime = 0;
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

    public void InitializeLaser(Vector3 position, Quaternion rotation, bool isFromPool = true)
    {
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
        currentLifetime = lifetime;
        ResetTrail();
    }

    public void SetLaserDamage(float damage)
    {
        laserDamage = damage;
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
        trackPlayer = true;
    }

    public void SetTarget(Transform target)
    {
        if (target != null) SetTargetPosition(target.position);
        else ClearTarget();
    }

    public void ClearTarget()
    {
        hasTarget = false;
        trackPlayer = false;
    }

    private void ResetTrail()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }

        if (prevStruckSensor != null)
        {
            LaserSensor.HandleLaser(this, prevStruckSensor, null);
            prevStruckSensor = null;
        }

        hasDoneDamage = false;
    }
}
