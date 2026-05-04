using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hardpoint game mode logic.
/// Teams compete to control capture zones, earning points over time.
/// First team to reach score limit wins.
/// </summary>
[RequireComponent(typeof(MatchManager))]
public class HardpointGameMode : MonoBehaviour
{
    [Header("Hardpoint Settings")]
    [SerializeField] private int _scoreLimit = 150;
    [SerializeField] private float _zoneDuration = 60f; // Time before zone rotates
    [SerializeField] private float _pointsPerSecond = 1f;
    [SerializeField] private bool _enableOvertime = true;
    
    [Header("Zones")]
    [SerializeField] private List<CaptureZone> _zones = new List<CaptureZone>();
    [SerializeField] private int _startZoneIndex = 0;
    
    private MatchManager _matchManager;
    private TeamManager _teamManager;
    private CaptureZone _currentZone;
    private int _currentZoneIndex;
    private float _zoneTimeRemaining;
    private bool _inOvertime;
    
    // Properties
    public CaptureZone CurrentZone => _currentZone;
    public float ZoneTimeRemaining => _zoneTimeRemaining;
    public int ScoreLimit => _scoreLimit;
    
    private void Awake()
    {
        _matchManager = GetComponent<MatchManager>();
    }
    
    private void Start()
    {
        _teamManager = _matchManager.TeamManager;
        
        if (_teamManager == null)
        {
            Debug.LogError("[HardpointGameMode] TeamManager not found!");
            return;
        }
        
        // Subscribe to events
        _matchManager.OnMatchStateChanged += OnMatchStateChanged;
        
        // Validate zones
        if (_zones.Count == 0)
        {
            Debug.LogError("[HardpointGameMode] No zones assigned!");
            return;
        }
        
        // Deactivate all zones initially
        foreach (CaptureZone zone in _zones)
        {
            if (zone != null)
            {
                zone.gameObject.SetActive(false);
            }
        }
    }
    
    private void OnDestroy()
    {
        if (_matchManager != null)
        {
            _matchManager.OnMatchStateChanged -= OnMatchStateChanged;
        }
        
        if (_currentZone != null)
        {
            _currentZone.OnControlChanged -= OnZoneControlChanged;
        }
    }
    
    private void Update()
    {
        if (_matchManager.CurrentState != MatchState.Playing) return;
        
        UpdateZoneRotation();
        UpdateScoring();
        CheckWinCondition();
    }
    
    private void OnMatchStateChanged(MatchStateData stateData)
    {
        if (stateData.State == MatchState.Playing)
        {
            StartMatch();
        }
    }
    
    private void StartMatch()
    {
        // Reset scores
        _teamManager.ResetScores();
        
        // Activate first zone
        _currentZoneIndex = _startZoneIndex;
        ActivateZone(_currentZoneIndex);
        
        _inOvertime = false;
        
        Debug.Log($"[Hardpoint] Match started! First to {_scoreLimit} points wins!");
    }
    
    private void ActivateZone(int index)
    {
        // Deactivate current zone
        if (_currentZone != null)
        {
            _currentZone.OnControlChanged -= OnZoneControlChanged;
            _currentZone.gameObject.SetActive(false);
        }
        
        // Clamp index
        index = Mathf.Clamp(index, 0, _zones.Count - 1);
        
        // Activate new zone
        _currentZone = _zones[index];
        if (_currentZone != null)
        {
            _currentZone.gameObject.SetActive(true);
            _currentZone.ResetZone();
            _currentZone.OnControlChanged += OnZoneControlChanged;
            
            _zoneTimeRemaining = _zoneDuration;
            
            Debug.Log($"[Hardpoint] Zone {index + 1} activated at {_currentZone.transform.position}");
        }
    }
    
    private void UpdateZoneRotation()
    {
        if (_currentZone == null) return;
        
        _zoneTimeRemaining -= Time.deltaTime;
        
        if (_zoneTimeRemaining <= 0f)
        {
            // Rotate to next zone
            _currentZoneIndex = (_currentZoneIndex + 1) % _zones.Count;
            ActivateZone(_currentZoneIndex);
        }
    }
    
    private void UpdateScoring()
    {
        if (_currentZone == null) return;
        
        // Only award points if zone is controlled (not contested)
        if (_currentZone.CurrentState == CaptureZone.ZoneState.Controlled)
        {
            Team controllingTeam = _currentZone.ControllingTeam;
            
            if (controllingTeam != Team.None)
            {
                // Award points (using score as points, not kills)
                float pointsToAdd = _pointsPerSecond * Time.deltaTime;
                
                TeamData teamData = _teamManager.GetTeam(controllingTeam);
                if (teamData != null)
                {
                    // Add fractional points
                    teamData.score += (int)pointsToAdd;
                    
                    // Clamp to score limit
                    if (teamData.score > _scoreLimit)
                    {
                        teamData.score = _scoreLimit;
                    }
                }
            }
        }
    }
    
    private void CheckWinCondition()
    {
        TeamData team1 = _teamManager.GetTeam(Team.Team1);
        TeamData team2 = _teamManager.GetTeam(Team.Team2);
        
        // Check score limit
        if (team1.score >= _scoreLimit)
        {
            _matchManager.EndMatch(Team.Team1, $"Reached {_scoreLimit} points");
        }
        else if (team2.score >= _scoreLimit)
        {
            _matchManager.EndMatch(Team.Team2, $"Reached {_scoreLimit} points");
        }
        
        // Check time expiration
        if (_matchManager.MatchTimeRemaining <= 0f && !_inOvertime)
        {
            HandleTimeExpired();
        }
    }
    
    private void HandleTimeExpired()
    {
        TeamData team1 = _teamManager.GetTeam(Team.Team1);
        TeamData team2 = _teamManager.GetTeam(Team.Team2);
        
        // Determine if overtime needed
        if (_enableOvertime && _currentZone != null)
        {
            // Check if losing team is on point
            Team losingTeam = team1.score < team2.score ? Team.Team1 : Team.Team2;
            Team controllingTeam = _currentZone.ControllingTeam;
            
            if (controllingTeam == losingTeam || _currentZone.GetTeamCountInZone(losingTeam) > 0)
            {
                // Overtime!
                _inOvertime = true;
                Debug.Log("[Hardpoint] OVERTIME! Losing team on point!");
                return;
            }
        }
        
        // No overtime, determine winner by score
        if (team1.score > team2.score)
        {
            _matchManager.EndMatch(Team.Team1, "Most points when time expired");
        }
        else if (team2.score > team1.score)
        {
            _matchManager.EndMatch(Team.Team2, "Most points when time expired");
        }
        else
        {
            _matchManager.EndMatch(Team.None, "Draw - equal points");
        }
    }
    
    private void OnZoneControlChanged(Team newControllingTeam)
    {
        Debug.Log($"[Hardpoint] Zone control changed to {newControllingTeam}");
    }
}
