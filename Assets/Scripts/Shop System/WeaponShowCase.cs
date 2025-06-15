using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WeaponShowCase : MonoBehaviour
{
    [SerializeField] private ShopItemUI[] weaponItems;
    //make sure they are in same order
    [SerializeField] private GameObject[] statsPanels;
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponCostText;

    int weaponItemsLen;
    private float transitionDuration = 0.4f;
    private int currentIndex = 0;

    private bool isTransitioning = false;

    void Start()
    {
        weaponItemsLen = weaponItems.Length;
        updateData();
        foreach (GameObject panel in statsPanels)
        {
            panel.SetActive(false);
        }

        statsPanels[currentIndex].SetActive(true);
        if (weaponItems[currentIndex].IsFullyBought())
            weaponCostText.text = "Owned";
    }

    private void updateData()
    {
        weaponImage.sprite = weaponItems[currentIndex].icon;
        weaponNameText.text = weaponItems[currentIndex].GetItemName();
        if (weaponItems[currentIndex].IsFullyBought())
            weaponCostText.text = "Owned";
        else
            weaponCostText.text = $"Cost: {weaponItems[currentIndex].GetItemCost()}";
    }

    public void ShowNextWeapon()
    {
        if (isTransitioning) return;
        int nextIndex = (currentIndex + 1) % weaponItemsLen;
        StartCoroutine(SwapWeapon(nextIndex, -1));
    }

    public void ShowPreviousWeapon()
    {
        if (isTransitioning) return;
        int prevIndex = (currentIndex - 1 + weaponItemsLen) % weaponItemsLen;
        StartCoroutine(SwapWeapon(prevIndex, 1));
    }

    IEnumerator SwapWeapon(int newIndex, int direction)
    {
        isTransitioning = true;

        // Animate out
        float t = 0;
        Vector2 startPos = weaponImage.rectTransform.anchoredPosition;
        float startRot = 0;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float lerp = t / transitionDuration;

            weaponImage.rectTransform.anchoredPosition = Vector2.Lerp(startPos, startPos + new Vector2(-direction * 200, 0), lerp);
            //weaponImage.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(startRot, direction * 30, lerp));
            weaponImage.color = new Color(1, 1, 1, 1 - lerp);
            yield return null;
        }

        statsPanels[currentIndex].SetActive(false);
        currentIndex = newIndex;
        statsPanels[currentIndex].SetActive(true);
        updateData();

        weaponImage.rectTransform.anchoredPosition = startPos + new Vector2(direction * 200, 0);
        //weaponImage.rectTransform.rotation = Quaternion.Euler(0, 0, -direction * 30);
        weaponImage.color = new Color(1, 1, 1, 0);

        // Animate in
        t = 0;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float lerp = t / transitionDuration;

            weaponImage.rectTransform.anchoredPosition = Vector2.Lerp(weaponImage.rectTransform.anchoredPosition, startPos, lerp);
            //weaponImage.rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(-direction * 30, 0, lerp));
            weaponImage.color = new Color(1, 1, 1, lerp);
            yield return null;
        }

        isTransitioning = false;

    }

    public void OnBuyButtonClicked()
    {
        weaponItems[currentIndex].OnBuyButtonClicked();
        if (weaponItems[currentIndex].IsFullyBought())
        {
            weaponCostText.text = $"Owned";
        }
    }
}
