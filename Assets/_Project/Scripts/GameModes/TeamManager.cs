using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Team manager for multiplayer matches.
/// Manages teams, scores, and team members.
/// </summary>
public class TeamManager : MonoBehaviour
{
    [Header("Team Configuration")]
    [SerializeField] private TeamData _team1Data;
    [SerializeField] private TeamData _team2Data;
    
    // Team members tracking
    private Dictionary<Team, List<TeamMember>> _teamMembers;
    
    // Events
    public event Action<Team, int> OnTeamScoreChanged;
    public event Action<Team, TeamMember> OnMemberJoinedTeam;
    public event Action<Team, TeamMember> OnMemberLeftTeam;
    
    // Properties
    public TeamData Team1Data => _team1Data;
    public TeamData Team2Data => _team2Data;
    
    private void Awake()
    {
        // Initialize teams
        if (_team1Data == null)
        {
            _team1Data = new TeamData(Team.Team1, "Blue Team", Color.blue);
        }
        
        if (_team2Data == null)
        {
            _team2Data = new TeamData(Team.Team2, "Red Team", Color.red);
        }
        
        _teamMembers = new Dictionary<Team, List<TeamMember>>
        {
            { Team.Team1, new List<TeamMember>() },
            { Team.Team2, new List<TeamMember>() }
        };
    }
    
    private void Start()
    {
        // Register existing team members
        RegisterAllTeamMembers();
    }
    
    private void RegisterAllTeamMembers()
    {
        TeamMember[] allMembers = FindObjectsByType<TeamMember>(FindObjectsSortMode.None);
        
        foreach (TeamMember member in allMembers)
        {
            RegisterTeamMember(member);
        }
        
        Debug.Log($"Team Manager: Registered {allMembers.Length} team members");
    }
    
    /// <summary>
    /// Register team member.
    /// </summary>
    public void RegisterTeamMember(TeamMember member)
    {
        if (member == null || member.Team == Team.None)
            return;
        
        if (!_teamMembers[member.Team].Contains(member))
        {
            _teamMembers[member.Team].Add(member);
            OnMemberJoinedTeam?.Invoke(member.Team, member);
            
            Debug.Log($"{member.name} joined {member.Team}");
        }
    }
    
    /// <summary>
    /// Unregister team member.
    /// </summary>
    public void UnregisterTeamMember(TeamMember member)
    {
        if (member == null || member.Team == Team.None)
            return;
        
        if (_teamMembers[member.Team].Contains(member))
        {
            _teamMembers[member.Team].Remove(member);
            OnMemberLeftTeam?.Invoke(member.Team, member);
        }
    }
    
    /// <summary>
    /// Add kill to team.
    /// </summary>
    public void AddKill(Team team)
    {
        GetTeamData(team)?.AddKill();
        OnTeamScoreChanged?.Invoke(team, GetTeamScore(team));
    }
    
    /// <summary>
    /// Add death to team.
    /// </summary>
    public void AddDeath(Team team)
    {
        GetTeamData(team)?.AddDeath();
    }
    
    /// <summary>
    /// Get team data.
    /// </summary>
    public TeamData GetTeamData(Team team)
    {
        switch (team)
        {
            case Team.Team1:
                return _team1Data;
            case Team.Team2:
                return _team2Data;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// Get team (alias for GetTeamData).
    /// </summary>
    public TeamData GetTeam(Team team)
    {
        return GetTeamData(team);
    }
    
    /// <summary>
    /// Get team score.
    /// </summary>
    public int GetTeamScore(Team team)
    {
        return GetTeamData(team)?.score ?? 0;
    }
    
    /// <summary>
    /// Get team members.
    /// </summary>
    public List<TeamMember> GetTeamMembers(Team team)
    {
        if (_teamMembers.ContainsKey(team))
        {
            return _teamMembers[team];
        }
        
        return new List<TeamMember>();
    }
    
    /// <summary>
    /// Reset all teams.
    /// </summary>
    public void ResetTeams()
    {
        _team1Data?.Reset();
        _team2Data?.Reset();
        
        OnTeamScoreChanged?.Invoke(Team.Team1, 0);
        OnTeamScoreChanged?.Invoke(Team.Team2, 0);
    }
    
    /// <summary>
    /// Reset scores (alias for ResetTeams).
    /// </summary>
    public void ResetScores()
    {
        ResetTeams();
    }
}
