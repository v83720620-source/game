using UnityEngine;

namespace FlumpGame.Managers
{
    /// <summary>
    /// Singleton для управления текущим выбранным игровым режимом.
    /// Сохраняется между сценами.
    /// </summary>
    public class GameModeManager : MonoBehaviour
    {
        private static GameModeManager _instance;
        
        public static GameModeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameModeManager");
                    _instance = go.AddComponent<GameModeManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private Data.GameModeData _selectedGameMode;
        
        public Data.GameModeData SelectedGameMode => _selectedGameMode;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        /// <summary>
        /// Устанавливает выбранный игровой режим.
        /// </summary>
        public void SetSelectedGameMode(Data.GameModeData gameMode)
        {
            _selectedGameMode = gameMode;
            Debug.Log($"[GameModeManager] Game mode set to: {gameMode?.modeName ?? "NULL"}");
        }
        
        /// <summary>
        /// Очищает выбранный режим.
        /// </summary>
        public void ClearSelectedGameMode()
        {
            _selectedGameMode = null;
            Debug.Log("[GameModeManager] Game mode cleared");
        }
        
        /// <summary>
        /// Получает текущий режим игры как строку для отладки.
        /// </summary>
        public string GetCurrentModeInfo()
        {
            if (_selectedGameMode == null)
                return "No game mode selected";
            
            return $"{_selectedGameMode.modeName} - {_selectedGameMode.playersPerTeam}v{_selectedGameMode.playersPerTeam}";
        }
    }
}
