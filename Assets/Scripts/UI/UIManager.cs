using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI creditsText;

    private void Awake()
    {

    }
    private void Start()
    {
        ShopManager.Singelton.playerInventory.OnCreditsChanged += UpdateCreditsUI;
        UpdateCreditsUI(ShopManager.Singelton.playerInventory.Credits);
    }

    private void UpdateCreditsUI(int credits)
    {
        creditsText.text = $"Credits: {credits}";
    }

    private void OnDestroy()
    {
        if (ShopManager.Singelton.playerInventory != null)
        {
            ShopManager.Singelton.playerInventory.OnCreditsChanged -= UpdateCreditsUI;
        }
    }
}
