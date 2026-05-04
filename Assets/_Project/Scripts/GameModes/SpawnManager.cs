using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Spawn manager for players and bots.
/// Handles respawning and spawn point selection.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float _respawnDelay = 3f;
    [SerializeField] private bool _autoRespawn = true;
    
    [Header("Spawn Points")]
    [SerializeField] private SpawnPoint[] _spawnPoints;
    
    // Cached spawn points
    private Dictionary<Team, List<SpawnPoint>> _teamSpawnPoints;
    
    private void Awake()
    {
        // Find all spawn points if not assigned
        if (_spawnPoints == null || _spawnPoints.Length == 0)
        {
            _spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        }
        
        // Cache spawn points by team
        CacheSpawnPoints();
    }
    
    private void CacheSpawnPoints()
    {
        _teamSpawnPoints = new Dictionary<Team, List<SpawnPoint>>();
        
        foreach (SpawnPoint sp in _spawnPoints)
        {
            if (sp == null)
                continue;
            
            if (!_teamSpawnPoints.ContainsKey(sp.Team))
            {
                _teamSpawnPoints[sp.Team] = new List<SpawnPoint>();
            }
            
            _teamSpawnPoints[sp.Team].Add(sp);
        }
        
        Debug.Log($"Spawn Manager: Cached {_spawnPoints.Length} spawn points");
    }
    
    /// <summary>
    /// Get random spawn point for team.
    /// </summary>
    public SpawnPoint GetSpawnPoint(Team team, bool playerSpawn = true)
    {
        List<SpawnPoint> validSpawns = new List<SpawnPoint>();
        
        // Try team-specific spawns first
        if (_teamSpawnPoints.ContainsKey(team))
        {
            validSpawns = _teamSpawnPoints[team]
                .Where(sp => sp.IsPlayerSpawn == playerSpawn || !playerSpawn)
                .ToList();
        }
        
        // Fallback to neutral spawns
        if (validSpawns.Count == 0 && _teamSpawnPoints.ContainsKey(Team.None))
        {
            validSpawns = _teamSpawnPoints[Team.None]
                .Where(sp => sp.IsPlayerSpawn == playerSpawn || !playerSpawn)
                .ToList();
        }
        
        // Return random spawn
        if (validSpawns.Count > 0)
        {
            return validSpawns[Random.Range(0, validSpawns.Count)];
        }
        
        Debug.LogWarning($"No valid spawn point found for team {team}!");
        return null;
    }
    
    /// <summary>
    /// Spawn entity at spawn point.
    /// </summary>
    public void SpawnAt(GameObject entity, SpawnPoint spawnPoint)
    {
        if (entity == null || spawnPoint == null)
            return;
        
        entity.transform.position = spawnPoint.GetRandomPosition();
        entity.transform.rotation = spawnPoint.SpawnRotation;
        
        // Reset health
        PlayerHealth health = entity.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Respawn();
        }
        
        Debug.Log($"Spawned {entity.name} at {spawnPoint.name}");
    }
    
    /// <summary>
    /// Respawn entity after delay.
    /// </summary>
    public void RespawnAfterDelay(GameObject entity, Team team)
    {
        if (!_autoRespawn)
            return;
        
        StartCoroutine(RespawnCoroutine(entity, team));
    }
    
    private System.Collections.IEnumerator RespawnCoroutine(GameObject entity, Team team)
    {
        yield return new WaitForSeconds(_respawnDelay);
        
        SpawnPoint spawnPoint = GetSpawnPoint(team, true);
        if (spawnPoint != null)
        {
            SpawnAt(entity, spawnPoint);
        }
    }
}
