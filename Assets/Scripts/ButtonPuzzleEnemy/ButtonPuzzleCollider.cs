using UnityEngine;
using UnityEngine.AI;

public class ButtonPuzzleCollider : MonoBehaviour
{
    [SerializeField] NavMeshAgent[] agents;

    private void OnTriggerEnter(Collider other)
    {
        foreach (NavMeshAgent agent in agents) 
        {
            if (agent != null && other.gameObject == agent.gameObject)
            {
                agent.enabled = false;
                Debug.Log($"{agent.name} NavMeshAgent disabled.");
                break; // Stop checking once one is found
            }
        }
    }
}
