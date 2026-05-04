using System.Collections.Generic;
using UnityEngine;

namespace FlumpGame.Network.Bots
{
    /// <summary>
    /// Генератор реалистичных имён для ботов.
    /// </summary>
    public static class BotNameGenerator
    {
        private static readonly List<string> _firstNames = new List<string>
        {
            // Мужские имена
            "Alex", "Max", "John", "Mike", "David", "Chris", "Ryan", "Kevin", "Jason", "Tom",
            "Nick", "Sam", "Dan", "Jack", "Luke", "Mark", "Steve", "Ben", "Jake", "Kyle",
            "Matt", "Eric", "Adam", "Tyler", "Josh", "Brian", "Aaron", "Sean", "Brad", "Jeff",
            // Женские имена
            "Emma", "Olivia", "Ava", "Sophia", "Isabella", "Mia", "Charlotte", "Amelia", "Harper", "Evelyn",
            "Abigail", "Emily", "Lily", "Madison", "Chloe", "Grace", "Ella", "Victoria", "Scarlett", "Zoey",
            // Gaming nicknames
            "Shadow", "Phantom", "Ghost", "Hunter", "Sniper", "Reaper", "Viper", "Raven", "Wolf", "Tiger",
            "Dragon", "Phoenix", "Ninja", "Samurai", "Knight", "Viking", "Warrior", "Striker", "Blaze", "Storm"
        };
        
        private static readonly List<string> _lastNames = new List<string>
        {
            // Обычные фамилии
            "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor",
            "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson",
            "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "King", "Wright",
            // Gaming suffixes
            "Killer", "Slayer", "Master", "Pro", "Legend", "Elite", "Ace", "Boss", "King", "Lord",
            "God", "Destroyer", "Crusher", "Terminator", "Eliminator", "Dominator", "Champion", "Winner", "Hero", "Star"
        };
        
        private static readonly List<string> _usedNames = new List<string>();
        
        /// <summary>
        /// Генерирует случайное уникальное имя.
        /// </summary>
        public static string GenerateName()
        {
            const int maxAttempts = 100;
            
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                string name = GenerateRandomName();
                
                if (!_usedNames.Contains(name))
                {
                    _usedNames.Add(name);
                    return name;
                }
            }
            
            // Fallback: добавляем число
            string fallbackName = GenerateRandomName() + Random.Range(1, 999);
            _usedNames.Add(fallbackName);
            return fallbackName;
        }
        
        /// <summary>
        /// Генерирует простое имя (без проверки уникальности).
        /// </summary>
        private static string GenerateRandomName()
        {
            int format = Random.Range(0, 3);
            
            switch (format)
            {
                case 0: // First + Last
                    return GetRandomFirstName() + GetRandomLastName();
                
                case 1: // First + Number
                    return GetRandomFirstName() + Random.Range(10, 99);
                
                case 2: // First_Last
                    return GetRandomFirstName() + "_" + GetRandomLastName();
                
                default:
                    return GetRandomFirstName() + GetRandomLastName();
            }
        }
        
        /// <summary>
        /// Генерирует имя с определённым стилем.
        /// </summary>
        public static string GenerateNameWithStyle(NameStyle style)
        {
            switch (style)
            {
                case NameStyle.Realistic:
                    return GetRandomFirstName() + " " + GetRandomLastName();
                
                case NameStyle.Gaming:
                    return GetRandomGamingName();
                
                case NameStyle.Short:
                    return GetRandomFirstName();
                
                case NameStyle.WithNumber:
                    return GetRandomFirstName() + Random.Range(1, 999);
                
                default:
                    return GenerateName();
            }
        }
        
        private static string GetRandomFirstName()
        {
            return _firstNames[Random.Range(0, _firstNames.Count)];
        }
        
        private static string GetRandomLastName()
        {
            return _lastNames[Random.Range(0, _lastNames.Count)];
        }
        
        private static string GetRandomGamingName()
        {
            // Gaming style: Shadow_Killer, PhantomMaster, etc.
            string first = _firstNames[Random.Range(_firstNames.Count - 20, _firstNames.Count)]; // Gaming nicknames
            string last = _lastNames[Random.Range(_lastNames.Count - 20, _lastNames.Count)]; // Gaming suffixes
            
            int format = Random.Range(0, 2);
            return format == 0 ? $"{first}_{last}" : $"{first}{last}";
        }
        
        /// <summary>
        /// Сбросить список использованных имён.
        /// </summary>
        public static void ResetUsedNames()
        {
            _usedNames.Clear();
        }
        
        /// <summary>
        /// Получить количество доступных уникальных комбинаций.
        /// </summary>
        public static int GetPossibleCombinations()
        {
            return _firstNames.Count * _lastNames.Count * 3; // 3 formats
        }
        
        public enum NameStyle
        {
            Random,     // Любой стиль
            Realistic,  // John Smith
            Gaming,     // ShadowKiller
            Short,      // Alex
            WithNumber  // Max99
        }
    }
}
