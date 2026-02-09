using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace FlumpGame.UI.MainMenu
{
    /// <summary>
    /// Управляет выбором игрового режима.
    /// </summary>
    public class GameModeSelector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _gameModeContainer;
        [SerializeField] private GameObject _gameModeCardPrefab;
        [SerializeField] private Button _backButton;
        
        [Header("Game Modes")]
        [SerializeField] private List<Data.GameModeData> _gameModes = new List<Data.GameModeData>();
        
        private List<GameObject> _spawnedCards = new List<GameObject>();
        private Data.GameModeData _selectedGameMode;
        
        private void Awake()
        {
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        private void OnEnable()
        {
            PopulateGameModes();
        }
        
        private void OnDisable()
        {
            ClearGameModes();
            
            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
        
        private void PopulateGameModes()
        {
            ClearGameModes();
            
            if (_gameModes == null || _gameModes.Count == 0)
            {
                Debug.LogWarning("[GameModeSelector] No game modes assigned!");
                return;
            }
            
            // Сортируем режимы по sortOrder
            var sortedModes = _gameModes
                .Where(mode => mode != null && mode.showInMenu)
                .OrderBy(mode => mode.sortOrder)
                .ToList();
            
            foreach (var gameMode in sortedModes)
            {
                CreateGameModeCard(gameMode);
            }
        }
        
        private void CreateGameModeCard(Data.GameModeData gameMode)
        {
            if (_gameModeCardPrefab == null || _gameModeContainer == null)
            {
                Debug.LogError("[GameModeSelector] Game mode card prefab or container is null!");
                return;
            }
            
            GameObject card = Instantiate(_gameModeCardPrefab, _gameModeContainer);
            _spawnedCards.Add(card);
            
            // Настраиваем карточку
            SetupCard(card, gameMode);
        }
        
        private void SetupCard(GameObject card, Data.GameModeData gameMode)
        {
            // Находим компоненты (поиск по имени, так как структура может отличаться)
            var texts = card.GetComponentsInChildren<TextMeshProUGUI>();
            var images = card.GetComponentsInChildren<Image>();
            Button button = card.GetComponent<Button>();
            
            // Простой вариант: ищем компоненты по порядку
            if (texts.Length >= 1)
                texts[0].text = gameMode.modeName;
            
            if (texts.Length >= 2)
                texts[1].text = gameMode.description;
            
            if (texts.Length >= 3)
            {
                // TODO: Получить реальное количество игроков онлайн
                int playersOnline = Random.Range(10, 50);
                texts[2].text = $"🟢 {playersOnline} players online";
            }
            
            // Устанавливаем иконку если есть
            if (gameMode.icon != null && images.Length > 1)
                images[1].sprite = gameMode.icon; // images[0] обычно background
            
            // Подписываемся на клик
            if (button != null)
            {
                button.onClick.AddListener(() => OnGameModeSelected(gameMode));
            }
        }
        
        private void ClearGameModes()
        {
            foreach (var card in _spawnedCards)
            {
                if (card != null)
                    Destroy(card);
            }
            
            _spawnedCards.Clear();
        }
        
        private void OnGameModeSelected(Data.GameModeData gameMode)
        {
            Debug.Log($"[GameModeSelector] Selected game mode: {gameMode.modeName}");
            _selectedGameMode = gameMode;
            
            // Сохраняем выбранный режим
            Managers.GameModeManager.Instance.SetSelectedGameMode(gameMode);
            
            // Переходим в Lobby
            SceneLoader.Instance.LoadScene("Lobby");
        }
        
        private void OnBackButtonClicked()
        {
            Debug.Log("[GameModeSelector] Back button clicked");
            
            // Возвращаемся в главное меню
            gameObject.SetActive(false);
            
            // Показываем главное меню
            MainMenuUI mainMenu = FindObjectOfType<MainMenuUI>();
            if (mainMenu != null)
            {
                mainMenu.ShowMainMenu();
            }
        }
    }
}
