using UnityEngine;

public class TogglePanelButton : MonoBehaviour
{
    [Header("Assign the panel to toggle")]
    [SerializeField] private GameObject panel;

    [Tooltip("Optional: Use this if you want to find the panel by name at runtime")]
    [SerializeField] private string panelName;

    public void TogglePanel()
    {
        if (panel == null && !string.IsNullOrEmpty(panelName))
        {
            panel = GameObject.Find(panelName);
        }

        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
        else
        {
            Debug.LogError("Panel to toggle is not assigned or found.");
        }
    }
}
