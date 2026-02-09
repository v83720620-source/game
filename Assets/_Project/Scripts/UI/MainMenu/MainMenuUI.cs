using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlumpGame.UI.MainMenu
{
    /// <summary>
    /// Управляет UI главного меню.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Main Menu Panels")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _gameModeSelectionPanel;
        
        [Header("Main Menu Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;
        
        [Header("Version Display")]
        [SerializeField] private TextMeshProUGUI _versionText;
        
        private void Awake()
        {
            SetupButtons();
            UpdateVersionText();
        }
        
        private void OnEnable()
        {
            ShowMainMenu();
        }
        
        private void SetupButtons()
        {
            if (_playButton != null)
                _playButton.onClick.AddListener(OnPlayButtonClicked);
            
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
        
        private void OnDisable()
        {
            if (_playButton != null)
                _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            
            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            
            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }
        
        private void UpdateVersionText()
        {
            if (_versionText != null)
            {
                _versionText.text = $"v{Application.version} Alpha";
            }
        }
        
        public void ShowMainMenu()
        {
            if (_mainMenuPanel != null)
                _mainMenuPanel.SetActive(true);
            
            if (_gameModeSelectionPanel != null)
                _gameModeSelectionPanel.SetActive(false);
        }
        
        private void OnPlayButtonClicked()
        {
            Debug.Log("[MainMenuUI] Play button clicked");
            
            // Показываем меню выбора режима игры
            if (_mainMenuPanel != null)
                _mainMenuPanel.SetActive(false);
            
            if (_gameModeSelectionPanel != null)
                _gameModeSelectionPanel.SetActive(true);
        }
        
        private void OnSettingsButtonClicked()
        {
            Debug.Log("[MainMenuUI] Settings button clicked");
            // TODO: Открыть меню настроек
        }
        
        private void OnQuitButtonClicked()
        {
            Debug.Log("[MainMenuUI] Quit button clicked");
            SceneLoader.Instance.QuitGame();
        }
    }
}
