using UnityEngine;

/// <summary>
/// Spawn point for players and bots.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Team _team = Team.None;
    [SerializeField] private bool _isPlayerSpawn = true;
    [SerializeField] private float _spawnRadius = 1f;
    
    // Properties
    public Team Team => _team;
    public bool IsPlayerSpawn => _isPlayerSpawn;
    public Vector3 SpawnPosition => transform.position;
    public Quaternion SpawnRotation => transform.rotation;
    
    /// <summary>
    /// Get random position within spawn radius.
    /// </summary>
    public Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
        return transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }
    
    /// <summary>
    /// Check if spawn point is valid for team.
    /// </summary>
    public bool IsValidForTeam(Team team)
    {
        return _team == Team.None || _team == team;
    }
    
    private void OnDrawGizmos()
    {
        // Draw spawn point
        Gizmos.color = TeamMember.GetTeamColor(_team);
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
        
        // Draw direction
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw spawn area
        Gizmos.color = new Color(TeamMember.GetTeamColor(_team).r, 
                                  TeamMember.GetTeamColor(_team).g, 
                                  TeamMember.GetTeamColor(_team).b, 
                                  0.3f);
        Gizmos.DrawSphere(transform.position, _spawnRadius);
    }
}
