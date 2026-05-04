using UnityEngine;

/// <summary>
/// Handles respawn when entity dies.
/// Links PlayerHealth with SpawnManager.
/// </summary>
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(TeamMember))]
public class RespawnHandler : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private bool _autoRespawn = true;
    [SerializeField] private float _respawnDelay = 3f;
    
    // Components
    private PlayerHealth _health;
    private TeamMember _teamMember;
    private SpawnManager _spawnManager;
    
    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
        _teamMember = GetComponent<TeamMember>();
    }
    
    private void Start()
    {
        // Find spawn manager
        _spawnManager = FindAnyObjectByType<SpawnManager>();
        
        if (_spawnManager == null)
        {
            Debug.LogWarning("RespawnHandler: SpawnManager not found!");
        }
        
        // Subscribe to death event
        if (_health != null)
        {
            _health.OnDeath += HandleDeath;
        }
    }
    
    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDeath -= HandleDeath;
        }
    }
    
    private void HandleDeath(GameObject killer)
    {
        if (!_autoRespawn || _spawnManager == null)
            return;
        
        // Start respawn
        StartCoroutine(RespawnCoroutine());
    }
    
    private System.Collections.IEnumerator RespawnCoroutine()
    {
        // Wait for respawn delay
        yield return new WaitForSeconds(_respawnDelay);
        
        // Find spawn point
        SpawnPoint spawnPoint = _spawnManager.GetSpawnPoint(_teamMember.Team, _teamMember.IsPlayer);
        
        if (spawnPoint != null)
        {
            // Teleport to spawn
            transform.position = spawnPoint.GetRandomPosition();
            transform.rotation = spawnPoint.SpawnRotation;
            
            // Respawn (restore health)
            _health.Respawn();
            
            Debug.Log($"{gameObject.name} respawned at {spawnPoint.name}");
        }
        else
        {
            Debug.LogError($"No spawn point found for {gameObject.name} (Team: {_teamMember.Team})");
        }
    }
}
