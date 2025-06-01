using TMPro;
using UnityEngine;

public class MoneyCount : MonoBehaviour
{
    public static MoneyCount instance;
    public TMP_Text scoreText;
    private int moneyCount = 0;
    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = "Money: " + moneyCount.ToString();
        }
    }

    public void AddMoney(int manageScore)
    {
        if (scoreText != null)
        {
            moneyCount += manageScore;
            scoreText.text = "Money: " + moneyCount.ToString();
        }
    }
}
