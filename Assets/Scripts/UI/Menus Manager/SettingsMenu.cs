using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    private SettingsShared currentOrigin;

    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainMenuPanel;  //Only assigned in Menu Scene
    [SerializeField] private GameObject pauseMenuPanel; //Only assigned in Game Scene
    [SerializeField] List<GameObject> panelList = new List<GameObject>();
    public void Open(SettingsShared origin)
    {
        currentOrigin = origin;
        settingsPanel.SetActive(true);
    }

    public void Close()
    {
        settingsPanel.SetActive(false);

        switch (currentOrigin)
        {
            case SettingsShared.MainMenu:
                if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
                break;
            case SettingsShared.PauseMenu:
                if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
                break;
        }
    }

    public void OnButtonClick(GameObject on)
    {

        foreach (GameObject panelOff in panelList)
        {
            panelOff.SetActive(false);
        }

        on.SetActive(true);
    }
}
