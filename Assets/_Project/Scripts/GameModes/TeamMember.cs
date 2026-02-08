using System;
using UnityEngine;

/// <summary>
/// Team member component.
/// Identifies which team this entity belongs to.
/// </summary>
public class TeamMember : MonoBehaviour
{
    [Header("Team Settings")]
    [SerializeField] private Team _team = Team.Team1;
    [SerializeField] private bool _isPlayer = false;
    
    [Header("Visual")]
    [SerializeField] private Renderer[] _teamColorRenderers;
    [SerializeField] private Material _teamMaterial;
    
    // Events
    public event Action<Team> OnTeamChanged;
    
    // Properties
    public Team Team => _team;
    public bool IsPlayer => _isPlayer;
    
    private void Start()
    {
        ApplyTeamColor();
    }
    
    /// <summary>
    /// Set team.
    /// </summary>
    public void SetTeam(Team team)
    {
        if (_team == team)
            return;
        
        _team = team;
        ApplyTeamColor();
        OnTeamChanged?.Invoke(team);
    }
    
    /// <summary>
    /// Check if this entity is on the same team as another.
    /// </summary>
    public bool IsSameTeam(TeamMember other)
    {
        if (other == null)
            return false;
        
        return _team == other._team && _team != Team.None;
    }
    
    /// <summary>
    /// Check if this entity is on the same team.
    /// </summary>
    public bool IsSameTeam(Team team)
    {
        return _team == team && _team != Team.None;
    }
    
    /// <summary>
    /// Apply team color to renderers.
    /// </summary>
    private void ApplyTeamColor()
    {
        if (_teamColorRenderers == null || _teamColorRenderers.Length == 0)
            return;
        
        Color teamColor = GetTeamColor(_team);
        
        foreach (Renderer renderer in _teamColorRenderers)
        {
            if (renderer != null)
            {
                if (_teamMaterial != null)
                {
                    Material mat = new Material(_teamMaterial);
                    mat.color = teamColor;
                    renderer.material = mat;
                }
                else
                {
                    renderer.material.color = teamColor;
                }
            }
        }
    }
    
    /// <summary>
    /// Get color for team.
    /// </summary>
    public static Color GetTeamColor(Team team)
    {
        switch (team)
        {
            case Team.Team1:
                return Color.blue;
            case Team.Team2:
                return Color.red;
            default:
                return Color.gray;
        }
    }
}
