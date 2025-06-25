using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    PlayerInventory inventory;

    

    private void Start()
    {
        inventory = SaveManager.Singleton.playerInventoryHolder.Inventory;
        inventory.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        inventory.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(float normalizedHealth)
    {
        if (healthText == null && fillImage == null)
        {
            Debug.LogError("Health Text && fillimage are not assigned!");
            return;
        }

        fillImage.fillAmount = normalizedHealth;

        int percent = Mathf.RoundToInt(normalizedHealth * 100);
        healthText.text = percent + "%";
    }
}
