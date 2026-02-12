using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using FlumpGame.Network;

namespace FlumpGame.UI.Lobby
{
    /// <summary>
    /// Управляет UI matchmaking (поиск игроков).
    /// Использует реальную QueueSystem вместо симуляции.
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
        
        // Queue System
        private QueueSystem _queueSystem;
        private bool _isSearching = false;
        
        private void Awake()
        {
            SetupButtons();
            
            // Создаём Queue System
            _queueSystem = new QueueSystem();
            _queueSystem.OnStateChanged += OnQueueStateChanged;
            _queueSystem.OnPlayersChanged += OnPlayersChanged;
            _queueSystem.OnMatchReady += OnMatchReady;
        }
        
        private void OnDestroy()
        {
            if (_queueSystem != null)
            {
                _queueSystem.OnStateChanged -= OnQueueStateChanged;
                _queueSystem.OnPlayersChanged -= OnPlayersChanged;
                _queueSystem.OnMatchReady -= OnMatchReady;
            }
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
                // Обновляем Queue System
                _queueSystem?.UpdateQueue();
                
                // UI обновления
                UpdateTimerUI();
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
            
            // Определяем количество игроков
            var selectedMode = Managers.GameModeManager.Instance.SelectedGameMode;
            int playersNeeded = 10; // Default
            
            if (selectedMode != null)
            {
                playersNeeded = selectedMode.playersPerTeam * 2;
            }
            
            // Запускаем реальную Queue System
            _queueSystem.StartSearch(playersNeeded);
            
            UpdateStatusText("Searching for players...");
            
            Debug.Log($"[MatchmakingUI] Started real matchmaking for {playersNeeded} players");
        }
        
        private void StopSearch()
        {
            _isSearching = false;
            _queueSystem?.StopSearch();
            StopAllCoroutines();
        }
        
        private void UpdateTimerUI()
        {
            if (_timerText != null && _queueSystem != null)
            {
                float time = _queueSystem.TimeInQueue;
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
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
        
        private void UpdatePlayersFoundText(int found, int needed)
        {
            if (_playersFoundText != null)
            {
                _playersFoundText.text = $"Players found: {found}/{needed}";
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
        // QUEUE SYSTEM CALLBACKS
        // ============================================
        
        private void OnQueueStateChanged(QueueSystem.QueueState newState)
        {
            Debug.Log($"[MatchmakingUI] Queue state: {newState}");
            
            switch (newState)
            {
                case QueueSystem.QueueState.Searching:
                    UpdateStatusText("Searching for players...");
                    break;
                
                case QueueSystem.QueueState.BotFilling:
                    UpdateStatusText("Adding bots...");
                    break;
                
                case QueueSystem.QueueState.Starting:
                    UpdateStatusText("Match starting!");
                    break;
                
                case QueueSystem.QueueState.Idle:
                    _isSearching = false;
                    break;
            }
        }
        
        private void OnPlayersChanged(int found, int needed)
        {
            UpdatePlayersFoundText(found, needed);
            Debug.Log($"[MatchmakingUI] Players: {found}/{needed}");
        }
        
        private void OnMatchReady()
        {
            Debug.Log("[MatchmakingUI] Match ready! Loading...");
            
            // Показываем список игроков
            ShowPlayerList();
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
