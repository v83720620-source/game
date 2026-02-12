using UnityEngine;

namespace FlumpGame.Network.Bots
{
    /// <summary>
    /// Данные бота для spawning.
    /// </summary>
    [System.Serializable]
    public class BotData
    {
        public string botName;
        public int teamId;
        public BotDifficulty difficulty;
        public int fakePing; // Для отображения в UI
        
        public BotData(string name, int team, BotDifficulty diff)
        {
            botName = name;
            teamId = team;
            difficulty = diff;
            fakePing = GenerateFakePing(diff);
        }
        
        private int GenerateFakePing(BotDifficulty difficulty)
        {
            // Генерируем реалистичный ping в зависимости от сложности
            return difficulty switch
            {
                BotDifficulty.Dummy => Random.Range(80, 150),    // Высокий ping для слабых ботов
                BotDifficulty.Easy => Random.Range(50, 100),
                BotDifficulty.Normal => Random.Range(30, 70),
                BotDifficulty.Hard => Random.Range(10, 40),      // Низкий ping для сильных ботов
                _ => Random.Range(20, 80)
            };
        }
    }
    
    /// <summary>
    /// Уровни сложности ботов.
    /// </summary>
    public enum BotDifficulty
    {
        Dummy = 0,      // Манекен (для обучения)
        Easy = 1,       // Новичок
        Normal = 2,     // Средний
        Hard = 3        // Ветеран
    }
}
