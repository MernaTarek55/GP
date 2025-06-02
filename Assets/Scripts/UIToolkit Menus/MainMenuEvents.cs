using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;

    private Button _StartGameButton;
    private Button _ExitGameButton;

    private VisualElement _root;

    private Label _titleLabel;

    [Header("Settings")]
    [SerializeField]
    private Button _SettingsButton;
    [SerializeField] private VisualTreeAsset _settingsTemplate;
    private VisualElement _settingsInstance;
    private VisualElement _buttonWrapper;
    private VisualElement _gameTitle;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;

        _titleLabel = _root.Q<Label>("Title");

        _StartGameButton = _root.Q<Button>("StartGame");
        _StartGameButton.RegisterCallback<ClickEvent>(OnPlayGameClick);

        _ExitGameButton = _root.Q<Button>("ExitGame");
        _ExitGameButton.RegisterCallback<ClickEvent>(OnExitGameClick);

        _SettingsButton = _root.Q<Button>("Settings");
        _SettingsButton.clicked += OnSettingsClick;

        _buttonWrapper = _root.Q<VisualElement>("ButtonWrapper");
        _gameTitle = _root.Q<VisualElement>("GameTitle");

        _settingsInstance = _settingsTemplate.CloneTree();
        var backButton = _settingsInstance.Q<Button>("BackButton");
        backButton.clicked += OnBackClick;
    }


    void OnPlayGameClick(ClickEvent evt)
    {
        SceneManager.LoadScene("Sprint2");
    }

    void OnExitGameClick(ClickEvent evt)
    {
        Application.Quit();
    }

    private void OnSettingsClick()
    {
        _buttonWrapper.Clear();
        _gameTitle.Clear();
        _buttonWrapper.Add(_settingsInstance);
    }

    private void OnBackClick()
    {
        _buttonWrapper.Clear();
        _buttonWrapper.Add(_StartGameButton);
        _buttonWrapper.Add(_SettingsButton);
        _buttonWrapper.Add(_ExitGameButton);
        _gameTitle.Add(_titleLabel);
    }
}
