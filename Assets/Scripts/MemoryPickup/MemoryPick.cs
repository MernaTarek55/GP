using System.Collections;
using UnityEngine;

public class MemoryPick : PickupBase
{
    [Header("Comic Settings")]
    [SerializeField] private GameObject comicPanel; // Assign your ComicPanel here
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private SOAnimationPresets comicAnimationPreset; // Assign your ComicPopupPreset

    private UIAutoAnimation comicAnimator;

    private void Awake()
    {
        if (comicPanel != null)
        {
            comicAnimator = comicPanel.GetComponent<UIAutoAnimation>();
            comicPanel.SetActive(false); // Start hidden
        }
    }

    public override void Pickup(GameObject player)
    {
        if (MemoryManager.Instance != null)
        {
            if (MemoryManager.Instance.PickingUpMemory(gameObject))
            {
                ShowComic();
                gameObject.GetComponent<Rigidbody>().useGravity = false; // Disable gravity for the memory object
                gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
                gameObject.GetComponent<Collider>().isTrigger = true; // Disable collider to prevent further interactions
            }
        }
    }

    private void ShowComic()
    {
        if (comicPanel != null && comicAnimator != null)
        {
            // Apply the animation preset
            comicAnimator.animationEntrancePresets = comicAnimationPreset;
            comicAnimator.animationExitPresets = comicAnimationPreset;

            // Show and animate the panel
            comicPanel.SetActive(true);
            comicAnimator.EntranceAnimation();

            // Hide after duration
            StartCoroutine(HideComicAfterDelay());
        }
    }

    private IEnumerator HideComicAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);

        if (comicPanel != null && comicAnimator != null)
        {
            comicAnimator.ExitAnimation();

            // Wait for exit animation to finish before hiding
            yield return new WaitForSeconds(comicAnimationPreset.alphaDuration);
            comicPanel.SetActive(false);
        }
        Destroy(gameObject); // Remove the memory after collection
    }
}