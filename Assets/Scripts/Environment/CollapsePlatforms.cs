using UnityEngine;

public class CollapsePlatforms : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
    if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
