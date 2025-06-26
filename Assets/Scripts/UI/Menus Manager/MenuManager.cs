using UnityEngine;
using UnityEngine.SceneManagement;
public static class GameStartType
{
    public static bool IsNewGame = true;
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private SceneLoader sceneLoader;
    private int lastLvlIndex;

    private void Start()
    {
        lastLvlIndex = SaveManager.Singleton.LastPlayedLevel;
    }
    public void PlayNewGame()
    {
        GameStartType.IsNewGame = true;
        SaveManager.Singleton.MakeGameReady();
        sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void ContinueGame()
    {
        GameStartType.IsNewGame = false;
        sceneLoader.LoadScene(lastLvlIndex);

        //added to test shop saving bug
        SaveManager.Singleton.LoadGame();
        SaveManager.Singleton.MakeGameReady();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        //menuPanel.SetActive(false);
        settingsMenu.Open(SettingsShared.MainMenu);
    }
}
