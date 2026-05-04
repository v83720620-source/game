using UnityEngine;

namespace FlumpGame.Data
{
    /// <summary>
    /// Типы игровых режимов.
    /// </summary>
    public enum GameModeType
    {
        Duel1v1,
        Team3v3TDM,
        Team5v5TDM,
        Hardpoint5v5,
        Practice
    }
    
    /// <summary>
    /// Данные игрового режима.
    /// </summary>
    [CreateAssetMenu(fileName = "GameMode_", menuName = "Flump/Game Mode Data")]
    public class GameModeData : ScriptableObject
    {
        [Header("Mode Info")]
        public string modeName = "Team Deathmatch";
        [TextArea(2, 4)]
        public string description = "Eliminate enemies to reach the score limit.";
        public GameModeType modeType = GameModeType.Team5v5TDM;
        public Sprite icon;
        
        [Header("Match Settings")]
        [Tooltip("Количество игроков в команде")]
        public int playersPerTeam = 5;
        
        [Tooltip("Лимит очков для победы")]
        public int scoreLimit = 50;
        
        [Tooltip("Длительность матча в секундах")]
        public float matchDurationSeconds = 600f; // 10 minutes
        
        [Tooltip("Включить овертайм если счёт равный")]
        public bool enableOvertime = true;
        
        [Header("Scene")]
        [Tooltip("Название сцены для этого режима")]
        public string gameSceneName = "Game";
        
        [Header("Display")]
        [Tooltip("Показывать режим в меню")]
        public bool showInMenu = true;
        
        [Tooltip("Порядок сортировки в списке режимов")]
        public int sortOrder = 0;
        
        // Свойства для удобного доступа
        public int TotalPlayers => playersPerTeam * 2;
        public float MatchDurationMinutes => matchDurationSeconds / 60f;
    }
}
