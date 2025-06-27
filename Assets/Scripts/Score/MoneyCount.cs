using TMPro;
using UnityEngine;

public class MoneyCount : MonoBehaviour
{
    public TMP_Text scoreText;
    private PlayerInventory playerInventory;

    private void Start()
    {
        // Get the reference
        playerInventory = PlayerInventoryHolder.instance.Inventory;

        // Subscribe to the credits changed event
        playerInventory.OnCreditsChanged += UpdateCreditsUI;

        // Optional: initialize UI
        UpdateCreditsUI(playerInventory.Credits);
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnCreditsChanged -= UpdateCreditsUI;
    }

    private void UpdateCreditsUI(int credits)
    {
        if (scoreText != null)
            scoreText.text = credits.ToString();
    }
}
