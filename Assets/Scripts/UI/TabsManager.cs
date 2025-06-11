using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    [System.Serializable]
    public class TabData
    {
        public Button tabButton;
        public GameObject tabPanel;
    }

    public RectTransform tabContentContainer;
    public List<TabData> tabs = new();

    private void Start()
    {
        foreach (TabData tab in tabs)
        {
            GameObject currentPanel = tab.tabPanel;
            tab.tabButton.onClick.AddListener(() =>
            {
                foreach (Transform child in tabContentContainer)
                {
                    child.gameObject.SetActive(false);
                }

                currentPanel.SetActive(true);
            });
        }
    }
}
