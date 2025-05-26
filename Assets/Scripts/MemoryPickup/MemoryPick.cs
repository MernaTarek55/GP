
using System.Collections;
using UnityEngine;

public class MemoryPick : PickupBase
{
    [SerializeField] GameObject WrongMemoryMessage;
    [SerializeField] float messageDuration = 3.0f;
    public override void Pickup(GameObject player)
    {
        if (MemoryManager.Instance != null)
        {
            if (MemoryManager.Instance.PickingUpMemory(this.gameObject))
            {
                Debug.Log($"Correct memory picked: {name}");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"Wrong memory: {name}. Pick the right one.");
                ShowWrongMemoryMessage();
            }
        }
    }

    private void ShowWrongMemoryMessage()
    {
        if (WrongMemoryMessage != null)
        {
            WrongMemoryMessage.SetActive(true);
            StartCoroutine(HideMessageAfterDelay());
        }
    }

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        if (WrongMemoryMessage != null)
        {
            WrongMemoryMessage.SetActive(false);
        }
    }
}
