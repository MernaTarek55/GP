using System.Collections.Generic;
using UnityEngine;
public enum PoolType
{
    Bullet,
    Laser,
    LavaProjectile,
    DropItem
}
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
     [System.Serializable]
    public class Pool
    {
        public PoolType type;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<PoolType, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // 🔹 Create a hidden parent object for organization
            GameObject poolParent = new GameObject(pool.type.ToString() + " Pool");
            poolParent.transform.parent = this.transform;
            //poolParent.hideFlags = HideFlags.HideInHierarchy;

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, poolParent.transform); // 👈 Set parent

                obj.SetActive(false);

                

                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.type, objectPool);
        }
    }



    public GameObject SpawnFromPool(PoolType type, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning("Pool with type " + type + " doesn't exist.");
            return null;
        }

        GameObject obj = poolDictionary[type].Count > 0 ? poolDictionary[type].Dequeue() : Instantiate(GetPrefabByTag(type));
        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }

    public void ReturnToPool(PoolType type, GameObject obj)
    {
        obj.SetActive(false);
        poolDictionary[type].Enqueue(obj);
    }

    public GameObject GetPrefabByTag(PoolType type)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning("Pool with type " + type + " doesn't exist.");
            return null;
        }

        foreach (GameObject obj in poolDictionary[type])
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        return null;
    }
}
