using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Capture zone for Hardpoint mode.
/// Detects players in zone and determines control.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CaptureZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private float _zoneRadius = 6f;
    [SerializeField] private float _captureRate = 1f; // Points per second
    
    [Header("References")]
    [SerializeField] private ZoneIndicator _indicator;
    
    // State
    private HashSet<TeamMember> _playersInZone = new HashSet<TeamMember>();
    private Team _controllingTeam = Team.None;
    private ZoneState _currentState = ZoneState.Neutral;
    private float _captureProgress = 0f; // 0-100
    
    // Events
    public event Action<Team> OnControlChanged;
    public event Action<ZoneState> OnStateChanged;
    public event Action<float> OnProgressChanged;
    
    // Properties
    public Team ControllingTeam => _controllingTeam;
    public ZoneState CurrentState => _currentState;
    public float CaptureProgress => _captureProgress;
    public float CaptureRate => _captureRate;
    public float ZoneRadius => _zoneRadius;
    
    public enum ZoneState
    {
        Neutral,    // No one in zone
        Capturing,  // One team capturing
        Controlled, // One team has control
        Contested   // Both teams in zone
    }
    
    private void Start()
    {
        // Auto-find indicator if not assigned
        if (_indicator == null)
        {
            _indicator = GetComponentInChildren<ZoneIndicator>();
        }
        
        // Setup collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
            
            // Setup sphere collider radius
            if (col is SphereCollider sphere)
            {
                sphere.radius = _zoneRadius;
            }
        }
        
        UpdateZoneVisuals();
    }
    
    private void Update()
    {
        UpdateZoneState();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        TeamMember member = other.GetComponent<TeamMember>();
        if (member != null && member.Team != Team.None)
        {
            _playersInZone.Add(member);
            Debug.Log($"[CaptureZone] {other.name} ({member.Team}) entered zone");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        TeamMember member = other.GetComponent<TeamMember>();
        if (member != null)
        {
            _playersInZone.Remove(member);
            Debug.Log($"[CaptureZone] {other.name} left zone");
        }
    }
    
    private void UpdateZoneState()
    {
        // Clean up dead/null players
        _playersInZone.RemoveWhere(member => 
        {
            if (member == null) return true;
            if (!member.TryGetComponent<PlayerHealth>(out var health)) return false;
            return health.IsDead;
        });
        
        // Count players per team
        int team1Count = 0;
        int team2Count = 0;
        
        foreach (TeamMember member in _playersInZone)
        {
            if (member.Team == Team.Team1) team1Count++;
            else if (member.Team == Team.Team2) team2Count++;
        }
        
        // Determine new state
        ZoneState newState = _currentState;
        Team newControllingTeam = _controllingTeam;
        
        if (team1Count > 0 && team2Count > 0)
        {
            // Both teams present - Contested
            newState = ZoneState.Contested;
        }
        else if (team1Count > 0)
        {
            // Only Team1 present
            newState = ZoneState.Capturing;
            newControllingTeam = Team.Team1;
        }
        else if (team2Count > 0)
        {
            // Only Team2 present
            newState = ZoneState.Capturing;
            newControllingTeam = Team.Team2;
        }
        else
        {
            // No one present
            if (_controllingTeam != Team.None)
            {
                newState = ZoneState.Controlled; // Still controlled, just empty
            }
            else
            {
                newState = ZoneState.Neutral;
            }
        }
        
        // Update capture progress
        if (newState == ZoneState.Capturing)
        {
            // Increase progress towards new controlling team
            if (newControllingTeam != _controllingTeam)
            {
                _captureProgress += Time.deltaTime * 10f; // 10 seconds to capture
                
                if (_captureProgress >= 100f)
                {
                    _captureProgress = 100f;
                    newState = ZoneState.Controlled;
                    
                    // Control changed
                    if (_controllingTeam != newControllingTeam)
                    {
                        _controllingTeam = newControllingTeam;
                        OnControlChanged?.Invoke(_controllingTeam);
                        Debug.Log($"[CaptureZone] Control changed to {_controllingTeam}");
                    }
                }
            }
            else if (newControllingTeam == _controllingTeam && _captureProgress < 100f)
            {
                // Already controlling, finish capture
                _captureProgress += Time.deltaTime * 10f;
                _captureProgress = Mathf.Min(_captureProgress, 100f);
            }
        }
        else if (newState == ZoneState.Contested)
        {
            // Contested - no progress change
        }
        
        // State changed
        if (_currentState != newState)
        {
            _currentState = newState;
            OnStateChanged?.Invoke(_currentState);
            UpdateZoneVisuals();
        }
        
        OnProgressChanged?.Invoke(_captureProgress);
    }
    
    private void UpdateZoneVisuals()
    {
        if (_indicator != null)
        {
            _indicator.UpdateVisuals(_currentState, _controllingTeam);
        }
    }
    
    /// <summary>
    /// Reset zone to neutral state.
    /// </summary>
    public void ResetZone()
    {
        _controllingTeam = Team.None;
        _currentState = ZoneState.Neutral;
        _captureProgress = 0f;
        _playersInZone.Clear();
        
        UpdateZoneVisuals();
    }
    
    /// <summary>
    /// Get number of players from a team in zone.
    /// </summary>
    public int GetTeamCountInZone(Team team)
    {
        int count = 0;
        foreach (TeamMember member in _playersInZone)
        {
            if (member.Team == team) count++;
        }
        return count;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw zone radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _zoneRadius);
        
        // Draw zone cylinder
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_zoneRadius * 2, 1, _zoneRadius * 2));
    }
}
