using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
public class DebugerONSCrenn : MonoBehaviour
{
    public static DebugerONSCrenn Debuuging;
    [SerializeField] TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Debuuging = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Log(string message)
    {
        text.text += message + "\n";
    }
}
