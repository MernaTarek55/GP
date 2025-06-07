using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private GameObject menuPanel;

    public void PlayGameButton()
    {
        SceneManager.LoadScene("PauseMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        menuPanel.SetActive(false);
        settingsMenu.Open(SettingsShared.MainMenu);
    }
}
