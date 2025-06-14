using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerInventoryHolder playerInventoryHolder;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;


    

    private void Start()
    {
        if (playerInventoryHolder == null) playerInventoryHolder = FindAnyObjectByType<PlayerInventoryHolder>();
        playerInventoryHolder.Inventory.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        playerInventoryHolder.Inventory.OnHealthChanged -= UpdateHealthUI;
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
