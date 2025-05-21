using System.Collections.Generic;
using UnityEngine;

public class DropItemPool : MonoBehaviour
{
    public static DropItemPool Instance;

    [Header("Pool Settings")]
    [SerializeField] private GameObject dropPrefab;  // Your single drop prefab
    [SerializeField] private int poolSize = 20;     // Number of objects to pre-instantiate

    private Queue<GameObject> objectPool;

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        objectPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(dropPrefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    public GameObject GetDropFromPool(Vector3 position, Quaternion rotation)
    {
        if (objectPool.Count == 0)
        {
            Debug.LogWarning("Pool exhausted! Creating new drop instance.");
            return Instantiate(dropPrefab, position, rotation);
        }

        GameObject drop = objectPool.Dequeue();
        drop.SetActive(true);
        drop.transform.position = position;
        drop.transform.rotation = rotation;

        return drop;
    }

    public void ReturnToPool(GameObject drop)
    {
        drop.SetActive(false);
        objectPool.Enqueue(drop);
    }
}