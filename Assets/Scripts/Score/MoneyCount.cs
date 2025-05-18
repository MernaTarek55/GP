using TMPro;
using UnityEngine;

public class MoneyCount : MonoBehaviour
{
    public static MoneyCount instance;
    public TMP_Text scoreText;
    int moneyCount = 0;
    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "Money: " + moneyCount.ToString();
    }

    public void AddMoney(int manageScore)
    {
        moneyCount += manageScore;
        scoreText.text = "Money: " + moneyCount.ToString();
    }
}
