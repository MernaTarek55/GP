using UnityEngine;

public class EnemyDropHandler : MonoBehaviour
{
    [SerializeField] private GameObject drop;
    [SerializeField] private Transform dropSpawnPoint;

    public void SpawnDrops()
    {
       
           
                SpawnSingleDrop(drop);
    }

  
    private void SpawnSingleDrop(GameObject itemPrefab)
    {
        // ... implementation ...
    }
}