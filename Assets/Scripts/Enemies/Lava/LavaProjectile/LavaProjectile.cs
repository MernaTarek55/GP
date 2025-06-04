using UnityEngine;

public class LavaProjectile : MonoBehaviour
{

    private QuadraticCurve quadraticCurve;

    [System.Serializable]
    public struct Range4
    {
        public float minX, maxX, minZ, maxZ;


        public Range4(float minX, float maxX, float minY, float maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minZ = minY;
            this.maxZ = maxY;
        }
    }


    // For different enemy types (for now Random shooting or target shooting)
    public enum ProjectileEnemyType
    {
        Target
        , Random
    }
    [SerializeField] private float speed = 1f;
    [Header("Positions")]
    //public GameObject LavaRobot;
    private Vector3 initialA;
    private Vector3 initialControl;
    private Vector3 initialB;
    [SerializeField] private Range4 randomRange; // (minX, maxX, minZ, maxZ)
    private Vector2 randomizer;
    [Header("References")]
    private  QuadraticCurve curve;
    private  Vector3 enemyPosition;

    public ProjectileEnemyType projectileType;


    private float _sampleTime;


    private void OnEnable()
    {
        CheckingForEmptyReferences();

        StoringProjectPositions(); // Storing positions of start and end points of the projectile path
        /**/
        // Initialize movement
        _sampleTime = 0f;
       // transform.position = initialA;
    }

    private void CheckingForEmptyReferences()
    {
        //if (LavaRobot == null)
        //{
        //    Debug.LogError("LavaRobot reference is missing!");
        //    ReturnToPool();
        //    return;
        //}

        if (quadraticCurve != null)
            curve = quadraticCurve;
        if (curve.Control == null)
        {
            Debug.LogError("Missing QuadraticCurve control point!");
            ReturnToPool();
            return;
        }
        if (curve == null)

        {
            Debug.LogError("Missing QuadraticCurve ");
            ReturnToPool();
            return;
        }
        if (curve.A == null || curve.B == null)
        {
            Debug.LogError("Missing Elements from quadratic curve!");
            ReturnToPool();
            return;
        }
    }

    private void StoringProjectPositions()
    {
        // Cache positions at activation
        initialA = curve.A.position;
        Debug.Log("Projectile A: " + initialA);
        Debug.Log("curve a: " + curve.A.position);
        initialControl = curve.Control.position;
        //change if u want to change target
        randomizer.x = Random.Range(randomRange.minX, randomRange.maxX);
        randomizer.y = Random.Range(randomRange.minZ, randomRange.maxZ);



        switch (projectileType)
        {
            case ProjectileEnemyType.Random:
                initialB = new Vector3(enemyPosition.x+randomizer.x, curve.B.position.y, enemyPosition.z+randomizer.y);
                //Debug.LogWarning(initialB);
                //Debug.Log("Projectile Random");
                break;
            case ProjectileEnemyType.Target:
                initialB = curve.B.position;
                //Debug.Log("Projectile Target");
                break;

        }
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        _sampleTime += Time.deltaTime * speed;
        _sampleTime = Mathf.Clamp01(_sampleTime);

        Vector3 position = Evaluate(_sampleTime);
        transform.position = position;

        // Calculate direction with adaptive look-ahead
        float lookAhead = Mathf.Max(0.001f, Mathf.Min(0.1f, (1f - _sampleTime) * 0.5f));
        Vector3 nextPosition = Evaluate(_sampleTime + lookAhead);



        Debug.DrawLine(initialA, initialControl, Color.red); // A to Control point
        Debug.DrawLine(initialControl, initialB, Color.red); // Control to B point

        Vector3 direction = nextPosition - position;
        if (direction.sqrMagnitude > 0.0001f) // More precise zero check
        {
            transform.forward = direction.normalized;
        }

        if (_sampleTime >= 1f)
        {
           //if(projectileType == ProjectileEnemyType.Target)
           //     ReturnToPool();
        }
    }

    private Vector3 Evaluate(float t)
    {
        t = Mathf.Clamp01(t);
        Vector3 ac = Vector3.Lerp(initialA, initialControl, t);
        Vector3 cb = Vector3.Lerp(initialControl, initialB, t);
        return Vector3.Lerp(ac, cb, t);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealthComponent>().TakeDamage(10);
        }


        ReturnToPool();
    }
    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        PoolManager.Instance?.ReturnToPool(PoolType.LavaProjectile,gameObject);
    }

   

    public void SetQuadraticCurve(QuadraticCurve quadraticCurve)
    {
        this.quadraticCurve = quadraticCurve;
    }
    public void SetenemyPosition(Vector3 enemyPos)
    {
        this.enemyPosition = enemyPos;
    }

}