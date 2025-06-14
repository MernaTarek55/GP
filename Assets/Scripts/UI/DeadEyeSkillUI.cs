using UnityEngine;
using UnityEngine.UI;
using static Sentry.MeasurementUnit;
public class DeadEyeSkillUI : MonoBehaviour
{

    [SerializeField] private Image deadEyeCooldownImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCooldownUI(1);
        UpdateCooldownUI(1 - (timer / duration));
    }
    //called to show duration or cooldown
    private void UpdateCooldownUI(float fillAmount)
    {
        if (deadEyeCooldownImage != null)
        {
            deadEyeCooldownImage.fillAmount = fillAmount;
        }
    }
}
