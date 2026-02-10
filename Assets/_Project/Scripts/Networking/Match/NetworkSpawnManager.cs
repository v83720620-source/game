using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FlumpGame.Network.Player;

namespace FlumpGame.Network.Match
{
    /// <summary>
    /// Управляет спавном игроков по сети.
    /// </summary>
    public class NetworkSpawnManager : NetworkBehaviour
    {
        private static NetworkSpawnManager _instance;
        public static NetworkSpawnManager Instance => _instance;
        
        [Header("Spawn Points")]
        [SerializeField] private Transform[] _team1SpawnPoints;
        [SerializeField] private Transform[] _team2SpawnPoints;
        [SerializeField] private Transform[] _neutralSpawnPoints; // Для Duel
        
        [Header("Spawn Settings")]
        [SerializeField] private float _minSpawnDistance = 10f; // Минимальная дистанция от врагов
        [SerializeField] private bool _randomSpawn = true;
        
        private Dictionary<ulong, Vector3> _lastSpawnPositions = new Dictionary<ulong, Vector3>();
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }
        
        private void Start()
        {
            // Auto-setup spawn points if not assigned
            if (_team1SpawnPoints == null || _team1SpawnPoints.Length == 0)
            {
                AutoSetupSpawnPoints();
            }
        }
        
        /// <summary>
        /// Получить позицию спавна для игрока.
        /// </summary>
        public Vector3 GetSpawnPosition(ulong clientId, int teamId)
        {
            Transform[] spawnPoints = GetSpawnPointsForTeam(teamId);
            
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning($"[NetworkSpawnManager] No spawn points for team {teamId}!");
                return Vector3.up; // Default spawn
            }
            
            // Random spawn
            if (_randomSpawn)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Vector3 position = spawnPoints[randomIndex].position;
                
                _lastSpawnPositions[clientId] = position;
                return position;
            }
            
            // Sequential spawn
            int index = (int)(clientId % (ulong)spawnPoints.Length);
            Vector3 spawnPos = spawnPoints[index].position;
            
            _lastSpawnPositions[clientId] = spawnPos;
            return spawnPos;
        }
        
        /// <summary>
        /// Получить безопасную позицию спавна (далеко от врагов).
        /// </summary>
        public Vector3 GetSafeSpawnPosition(ulong clientId, int teamId)
        {
            Transform[] spawnPoints = GetSpawnPointsForTeam(teamId);
            
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                return Vector3.up;
            }
            
            // Найти все точки спавна отсортированные по безопасности
            var safeSpawns = spawnPoints
                .OrderByDescending(sp => GetSpawnSafetyScore(sp.position, teamId))
                .ToArray();
            
            Vector3 position = safeSpawns[0].position;
            _lastSpawnPositions[clientId] = position;
            
            return position;
        }
        
        /// <summary>
        /// Получить rotation для спавна.
        /// </summary>
        public Quaternion GetSpawnRotation(ulong clientId, int teamId)
        {
            Transform[] spawnPoints = GetSpawnPointsForTeam(teamId);
            
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                return Quaternion.identity;
            }
            
            // Найти соответствующую spawn point
            int index = _randomSpawn ? Random.Range(0, spawnPoints.Length) : (int)(clientId % (ulong)spawnPoints.Length);
            
            return spawnPoints[index].rotation;
        }
        
        private float GetSpawnSafetyScore(Vector3 position, int teamId)
        {
            float score = 100f;
            
            // Найти всех игроков
            var players = FindObjectsByType<NetworkPlayerController>(FindObjectsSortMode.None);
            
            foreach (var player in players)
            {
                if (!player.IsSpawned) continue;
                
                // Получить команду игрока
                int playerTeam = NetworkTeamManager.Instance?.GetPlayerTeam(player.OwnerClientId) ?? 0;
                
                // Если враг - снижаем score
                if (playerTeam != teamId && playerTeam != 0)
                {
                    float distance = Vector3.Distance(position, player.transform.position);
                    
                    if (distance < _minSpawnDistance)
                    {
                        score -= (1f - distance / _minSpawnDistance) * 50f;
                    }
                }
            }
            
            return score;
        }
        
        private Transform[] GetSpawnPointsForTeam(int teamId)
        {
            return teamId switch
            {
                1 => _team1SpawnPoints,
                2 => _team2SpawnPoints,
                0 => _neutralSpawnPoints, // Neutral (for Duel)
                _ => _team1SpawnPoints // Fallback
            };
        }
        
        /// <summary>
        /// Setup spawn points from scene (auto-find).
        /// </summary>
        public void AutoSetupSpawnPoints()
        {
            // Find all spawn points in scene
            GameObject team1Spawns = GameObject.Find("Team1Spawns");
            GameObject team2Spawns = GameObject.Find("Team2Spawns");
            GameObject neutralSpawns = GameObject.Find("NeutralSpawns");
            
            if (team1Spawns != null)
            {
                _team1SpawnPoints = team1Spawns.GetComponentsInChildren<Transform>()
                    .Where(t => t != team1Spawns.transform).ToArray();
                Debug.Log($"[NetworkSpawnManager] Found {_team1SpawnPoints.Length} Team 1 spawn points");
            }
            
            if (team2Spawns != null)
            {
                _team2SpawnPoints = team2Spawns.GetComponentsInChildren<Transform>()
                    .Where(t => t != team2Spawns.transform).ToArray();
                Debug.Log($"[NetworkSpawnManager] Found {_team2SpawnPoints.Length} Team 2 spawn points");
            }
            
            if (neutralSpawns != null)
            {
                _neutralSpawnPoints = neutralSpawns.GetComponentsInChildren<Transform>()
                    .Where(t => t != neutralSpawns.transform).ToArray();
                Debug.Log($"[NetworkSpawnManager] Found {_neutralSpawnPoints.Length} Neutral spawn points");
            }
            
            // Fallback: use existing SpawnPoints if found
            if ((_team1SpawnPoints == null || _team1SpawnPoints.Length == 0) &&
                (_team2SpawnPoints == null || _team2SpawnPoints.Length == 0))
            {
                var allSpawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
                if (allSpawnPoints.Length > 0)
                {
                    _neutralSpawnPoints = allSpawnPoints.Select(sp => sp.transform).ToArray();
                    Debug.Log($"[NetworkSpawnManager] Using {_neutralSpawnPoints.Length} generic spawn points");
                }
            }
        }
    }
}
