using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace FlumpGame.UI.Lobby
{
    /// <summary>
    /// Управляет UI matchmaking (поиск игроков).
    /// </summary>
    public class MatchmakingUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _searchPanel;
        [SerializeField] private GameObject _playerListPanel;
        
        [Header("Search Status")]
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private TextMeshProUGUI _playersFoundText;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private RectTransform _loadingSpinner;
        
        [Header("Buttons")]
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _backButton;
        
        [Header("Player List")]
        [SerializeField] private Transform _playerListContainer;
        [SerializeField] private GameObject _playerListItemPrefab;
        
        [Header("Settings")]
        [SerializeField] private float _spinnerRotationSpeed = 180f; // degrees per second
        
        private bool _isSearching = false;
        private float _searchTimer = 0f;
        private int _playersFound = 0;
        private int _totalPlayersNeeded = 10;
        
        private void Awake()
        {
            SetupButtons();
        }
        
        private void OnEnable()
        {
            ShowSearchPanel();
            StartSearch();
        }
        
        private void OnDisable()
        {
            StopSearch();
        }
        
        private void Update()
        {
            if (_isSearching)
            {
                UpdateSearchTimer();
                RotateLoadingSpinner();
            }
        }
        
        private void SetupButtons()
        {
            if (_cancelButton != null)
                _cancelButton.onClick.AddListener(OnCancelButtonClicked);
            
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        private void ShowSearchPanel()
        {
            if (_searchPanel != null)
                _searchPanel.SetActive(true);
            
            if (_playerListPanel != null)
                _playerListPanel.SetActive(false);
        }
        
        private void ShowPlayerListPanel()
        {
            if (_searchPanel != null)
                _searchPanel.SetActive(false);
            
            if (_playerListPanel != null)
                _playerListPanel.SetActive(true);
        }
        
        private void StartSearch()
        {
            _isSearching = true;
            _searchTimer = 0f;
            _playersFound = 0;
            
            // Определяем количество игроков
            var selectedMode = Managers.GameModeManager.Instance.SelectedGameMode;
            if (selectedMode != null)
            {
                _totalPlayersNeeded = selectedMode.playersPerTeam * 2;
            }
            
            UpdateStatusText("Searching for players...");
            UpdatePlayersFoundText();
            
            Debug.Log($"[MatchmakingUI] Starting search for {_totalPlayersNeeded} players");
            
            // TODO: Начать реальный поиск игроков (Этап 16)
            // Пока симулируем
            StartCoroutine(SimulateMatchmaking());
        }
        
        private void StopSearch()
        {
            _isSearching = false;
            StopAllCoroutines();
        }
        
        private void UpdateSearchTimer()
        {
            _searchTimer += Time.deltaTime;
            
            if (_timerText != null)
            {
                int minutes = Mathf.FloorToInt(_searchTimer / 60f);
                int seconds = Mathf.FloorToInt(_searchTimer % 60f);
                _timerText.text = $"{minutes:00}:{seconds:00}";
            }
        }
        
        private void RotateLoadingSpinner()
        {
            if (_loadingSpinner != null)
            {
                float rotation = _loadingSpinner.localEulerAngles.z;
                rotation -= _spinnerRotationSpeed * Time.deltaTime;
                _loadingSpinner.localEulerAngles = new Vector3(0, 0, rotation);
            }
        }
        
        private void UpdateStatusText(string status)
        {
            if (_statusText != null)
                _statusText.text = status;
        }
        
        private void UpdatePlayersFoundText()
        {
            if (_playersFoundText != null)
            {
                _playersFoundText.text = $"Players found: {_playersFound}/{_totalPlayersNeeded}";
            }
        }
        
        private void OnCancelButtonClicked()
        {
            Debug.Log("[MatchmakingUI] Cancel matchmaking");
            StopSearch();
            
            // Возвращаемся в главное меню
            SceneLoader.Instance.LoadScene("MainMenu");
        }
        
        private void OnBackButtonClicked()
        {
            OnCancelButtonClicked();
        }
        
        // ============================================
        // ВРЕМЕННАЯ СИМУЛЯЦИЯ MATCHMAKING (для теста)
        // TODO: Заменить на реальный matchmaking в Этапе 16
        // ============================================
        
        private IEnumerator SimulateMatchmaking()
        {
            yield return new WaitForSeconds(1f);
            
            // Симулируем поиск игроков
            for (int i = 1; i <= _totalPlayersNeeded; i++)
            {
                yield return new WaitForSeconds(Random.Range(1f, 3f));
                
                _playersFound = i;
                UpdatePlayersFoundText();
                
                Debug.Log($"[MatchmakingUI] Found {_playersFound}/{_totalPlayersNeeded} players");
                
                if (_playersFound >= _totalPlayersNeeded)
                {
                    UpdateStatusText("Match found!");
                    yield return new WaitForSeconds(1f);
                    
                    // Показываем список игроков
                    ShowPlayerList();
                    yield break;
                }
            }
        }
        
        private void ShowPlayerList()
        {
            ShowPlayerListPanel();
            PopulatePlayerList();
            
            // Через 5 секунд запускаем матч
            StartCoroutine(StartMatchCountdown());
        }
        
        private void PopulatePlayerList()
        {
            ClearPlayerList();
            
            var selectedMode = Managers.GameModeManager.Instance.SelectedGameMode;
            if (selectedMode == null) return;
            
            int playersPerTeam = selectedMode.playersPerTeam;
            
            // Team 1
            for (int i = 0; i < playersPerTeam; i++)
            {
                CreatePlayerListItem($"Player{i + 1}", Random.Range(1, 50), Random.Range(10, 80), 1);
            }
            
            // Team 2
            for (int i = 0; i < playersPerTeam; i++)
            {
                CreatePlayerListItem($"Enemy{i + 1}", Random.Range(1, 50), Random.Range(10, 80), 2);
            }
        }
        
        private void CreatePlayerListItem(string playerName, int level, int ping, int team)
        {
            if (_playerListItemPrefab == null || _playerListContainer == null)
                return;
            
            GameObject item = Instantiate(_playerListItemPrefab, _playerListContainer);
            
            // Настраиваем item
            var playerListItem = item.GetComponent<PlayerListItem>();
            if (playerListItem != null)
            {
                playerListItem.Setup(playerName, level, ping, team);
            }
        }
        
        private void ClearPlayerList()
        {
            if (_playerListContainer == null) return;
            
            foreach (Transform child in _playerListContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        private IEnumerator StartMatchCountdown()
        {
            Debug.Log("[MatchmakingUI] Starting match countdown");
            
            // Запускаем хост (сервер + клиент)
            var networkManager = Network.CustomNetworkManager.Instance;
            if (networkManager != null)
            {
                networkManager.StartHost();
                yield return new WaitForSeconds(1f); // Ждём запуска
            }
            
            for (int i = 5; i > 0; i--)
            {
                UpdateStatusText($"Starting in {i}...");
                yield return new WaitForSeconds(1f);
            }
            
            Debug.Log("[MatchmakingUI] Loading game scene");
            
            // Загружаем игровую сцену
            var selectedMode = Managers.GameModeManager.Instance.SelectedGameMode;
            if (selectedMode != null)
            {
                SceneLoader.Instance.LoadScene(selectedMode.gameSceneName);
            }
            else
            {
                Debug.LogError("[MatchmakingUI] No game mode selected!");
                SceneLoader.Instance.LoadScene("Game");
            }
        }
    }
}
