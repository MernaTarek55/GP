using UnityEngine;
public static class GameStartType
{
    public static bool IsNewGame = true;
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private SceneLoader sceneLoader; 
    [SerializeField] private string gameSceneName = "Sprint4";

    public void PlayNewGame()
    {
        GameStartType.IsNewGame = true;
        sceneLoader.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        GameStartType.IsNewGame = false;
        sceneLoader.LoadScene(gameSceneName);
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
