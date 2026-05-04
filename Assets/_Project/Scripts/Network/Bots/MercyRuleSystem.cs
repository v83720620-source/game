using System.Collections.Generic;
using UnityEngine;

namespace FlumpGame.Network.Bots
{
    /// <summary>
    /// Mercy Rule система для защиты игроков от фрустрации.
    /// Автоматически снижает сложность ботов если игрок слишком часто умирает.
    /// </summary>
    public class MercyRuleSystem : MonoBehaviour
    {
        private static MercyRuleSystem _instance;
        public static MercyRuleSystem Instance => _instance;
        
        [Header("Settings")]
        [SerializeField] private bool _enableMercyRule = true;
        [SerializeField] private int _deathThreshold = 3; // 3 смерти подряд
        [SerializeField] private float _timeWindow = 60f; // В течение 60s
        
        #pragma warning disable 0414
        [SerializeField] private int _difficultyDropLevels = 1; // -1 уровень сложности (TODO: использовать в ApplyMercyRule)
        #pragma warning restore 0414
        
        // Player death tracking
        private Dictionary<ulong, PlayerDeathTracker> _playerTrackers = new Dictionary<ulong, PlayerDeathTracker>();
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }
        
        /// <summary>
        /// Регистрирует смерть игрока.
        /// </summary>
        public void RegisterPlayerDeath(ulong victimClientId, ulong killerClientId)
        {
            if (!_enableMercyRule) return;
            
            // Ignore bot deaths
            if (victimClientId >= 1000) return;
            
            // Only track deaths from bots
            if (killerClientId < 1000) return; // Killed by another player - don't apply mercy
            
            // Get or create tracker
            if (!_playerTrackers.TryGetValue(victimClientId, out var tracker))
            {
                tracker = new PlayerDeathTracker();
                _playerTrackers[victimClientId] = tracker;
            }
            
            // Add death
            tracker.AddDeath(Time.time);
            
            // Check if mercy rule should apply
            int recentDeaths = tracker.GetRecentDeaths(_timeWindow);
            
            Debug.Log($"[MercyRule] Player {victimClientId}: {recentDeaths} deaths in last {_timeWindow}s");
            
            if (recentDeaths >= _deathThreshold)
            {
                Debug.Log($"[MercyRule] ⚠️ Mercy Rule triggered for player {victimClientId}! Reducing bot difficulty...");
                ApplyMercyRule(victimClientId, killerClientId);
                
                // Reset tracker
                tracker.Reset();
            }
        }
        
        /// <summary>
        /// Регистрирует kill игрока (отменяет Mercy Rule).
        /// </summary>
        public void RegisterPlayerKill(ulong killerClientId)
        {
            if (!_enableMercyRule) return;
            if (killerClientId >= 1000) return; // Ignore bot kills
            
            // Player got a kill - reset their tracker
            if (_playerTrackers.TryGetValue(killerClientId, out var tracker))
            {
                tracker.Reset();
                Debug.Log($"[MercyRule] Player {killerClientId} got a kill - mercy rule reset");
            }
        }
        
        /// <summary>
        /// Apply mercy rule - reduce bot difficulty.
        /// </summary>
        private void ApplyMercyRule(ulong victimClientId, ulong botClientId)
        {
            // Find the bot that killed the player
            var bot = FindBotByClientId(botClientId);
            if (bot == null)
            {
                Debug.LogWarning($"[MercyRule] Bot {botClientId} not found!");
                return;
            }
            
            // TODO: Reduce bot difficulty
            // bot.ReduceDifficulty(_difficultyDropLevels);
            
            Debug.Log($"[MercyRule] Reduced difficulty for bot '{bot.name}' (killed player {victimClientId} too many times)");
            
            // Notify player (subtle)
            // Don't show "Bot difficulty reduced" - players shouldn't know!
        }
        
        /// <summary>
        /// Find bot by client ID.
        /// </summary>
        private GameObject FindBotByClientId(ulong botClientId)
        {
            // Bots have clientId >= 1000
            // TODO: Get from NetworkBotSpawner tracking list
            return null;
        }
        
        /// <summary>
        /// Get death streak for player.
        /// </summary>
        public int GetDeathStreak(ulong clientId)
        {
            if (_playerTrackers.TryGetValue(clientId, out var tracker))
            {
                return tracker.GetRecentDeaths(_timeWindow);
            }
            return 0;
        }
        
        /// <summary>
        /// Reset mercy rule for player.
        /// </summary>
        public void ResetPlayerMercy(ulong clientId)
        {
            if (_playerTrackers.ContainsKey(clientId))
            {
                _playerTrackers[clientId].Reset();
            }
        }
        
        /// <summary>
        /// Clear all trackers (called when match ends).
        /// </summary>
        public void ClearAllTrackers()
        {
            _playerTrackers.Clear();
            Debug.Log("[MercyRule] All trackers cleared");
        }
    }
    
    /// <summary>
    /// Tracks player deaths over time.
    /// </summary>
    public class PlayerDeathTracker
    {
        private List<float> _deathTimes = new List<float>();
        
        public void AddDeath(float time)
        {
            _deathTimes.Add(time);
        }
        
        public int GetRecentDeaths(float timeWindow)
        {
            float currentTime = Time.time;
            int count = 0;
            
            foreach (float deathTime in _deathTimes)
            {
                if (currentTime - deathTime <= timeWindow)
                {
                    count++;
                }
            }
            
            return count;
        }
        
        public void Reset()
        {
            _deathTimes.Clear();
        }
    }
}
