using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TabsManager : MonoBehaviour
{
    [System.Serializable]
    public class TabData
    {
        public Button tabButton;
        public GameObject tabPanel;
    }

    public RectTransform tabContentContainer;
    public List<TabData> tabs = new List<TabData>();

    void Start()
    {
        foreach (var tab in tabs)
        {
            var currentPanel = tab.tabPanel;
            tab.tabButton.onClick.AddListener(() =>
            {
                foreach (Transform child in tabContentContainer)
                    child.gameObject.SetActive(false);

                currentPanel.SetActive(true);
            });
        }
    }
}
