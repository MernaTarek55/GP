using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerInventoryHolder playerInventoryHolder;
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        if(playerInventoryHolder == null) playerInventoryHolder =  FindAnyObjectByType<PlayerInventoryHolder>();
        playerInventoryHolder.Inventory.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        playerInventoryHolder.Inventory.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float normalizedHealth)
    {
        fillImage.fillAmount = normalizedHealth;
    }
}
