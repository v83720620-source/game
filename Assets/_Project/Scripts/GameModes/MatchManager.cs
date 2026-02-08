using System;
using UnityEngine;

/// <summary>
/// Manages the overall match flow, state transitions, and coordination between game systems.
/// Singleton pattern for global access.
/// </summary>
public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    [Header("Match Settings")]
    [SerializeField] private float _matchDuration = 600f; // 10 minutes in seconds
    [SerializeField] private float _startCountdown = 3f;
    [SerializeField] private bool _autoStartMatch = true;

    [Header("References")]
    [SerializeField] private TeamManager _teamManager;

    [Header("UI References")]
    [SerializeField] private MatchUI _matchUI;
    [SerializeField] private KillFeedUI _killFeedUI;
    [SerializeField] private MatchEndUI _matchEndUI;

    // State
    private MatchState _currentState = MatchState.Waiting;
    private float _matchTimeRemaining;
    private float _startCountdownRemaining;

    // Events
    public event Action<MatchStateData> OnMatchStateChanged;
    public event Action<GameObject, GameObject, Team, Team> OnPlayerKilled; // killer, victim, killerTeam, victimTeam

    // Properties
    public MatchState CurrentState => _currentState;
    public float MatchTimeRemaining => _matchTimeRemaining;
    public float MatchDuration => _matchDuration;
    public TeamManager TeamManager => _teamManager;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Auto-find TeamManager if not assigned
        if (_teamManager == null)
        {
            _teamManager = FindAnyObjectByType<TeamManager>();
        }
    }

    private void Start()
    {
        _matchTimeRemaining = _matchDuration;
        _startCountdownRemaining = _startCountdown;

        if (_autoStartMatch)
        {
            StartMatch();
        }
    }

    private void Update()
    {
        switch (_currentState)
        {
            case MatchState.Starting:
                UpdateStartCountdown();
                break;
            case MatchState.Playing:
                UpdateMatchTimer();
                break;
        }
    }

    /// <summary>
    /// Starts the match (begins countdown).
    /// </summary>
    public void StartMatch()
    {
        if (_currentState != MatchState.Waiting) return;

        ChangeState(MatchState.Starting);
        Debug.Log("Match starting in " + _startCountdown + " seconds...");
    }

    /// <summary>
    /// Ends the match with a winner.
    /// </summary>
    public void EndMatch(Team winningTeam, string reason)
    {
        if (_currentState == MatchState.Ending || _currentState == MatchState.Finished) return;

        ChangeState(MatchState.Ending);
        Debug.Log($"Match ended! Winner: {winningTeam}, Reason: {reason}");

        // Show match end UI
        if (_matchEndUI != null)
        {
            _matchEndUI.ShowMatchEnd(winningTeam, _teamManager.GetTeam(Team.Team1), _teamManager.GetTeam(Team.Team2), reason);
        }

        // Finish after delay
        Invoke(nameof(FinishMatch), 3f);
    }

    /// <summary>
    /// Called when a player kills another player.
    /// </summary>
    public void RegisterKill(GameObject killer, GameObject victim)
    {
        if (_currentState != MatchState.Playing) return;

        // Get teams
        Team killerTeam = Team.None;
        Team victimTeam = Team.None;

        TeamMember killerMember = killer != null ? killer.GetComponent<TeamMember>() : null;
        TeamMember victimMember = victim != null ? victim.GetComponent<TeamMember>() : null;

        if (killerMember != null) killerTeam = killerMember.Team;
        if (victimMember != null) victimTeam = victimMember.Team;

        // Notify listeners
        OnPlayerKilled?.Invoke(killer, victim, killerTeam, victimTeam);

        // Show in kill feed
        if (_killFeedUI != null)
        {
            string killerName = killer != null ? killer.name : "Unknown";
            string victimName = victim != null ? victim.name : "Unknown";
            _killFeedUI.AddKillEntry(killerName, victimName, killerTeam, victimTeam);
        }

        Debug.Log($"[MatchManager] {killer?.name} ({killerTeam}) killed {victim?.name} ({victimTeam})");
    }

    private void UpdateStartCountdown()
    {
        _startCountdownRemaining -= Time.deltaTime;

        if (_startCountdownRemaining <= 0f)
        {
            ChangeState(MatchState.Playing);
            Debug.Log("Match started!");
        }
    }

    private void UpdateMatchTimer()
    {
        _matchTimeRemaining -= Time.deltaTime;

        if (_matchTimeRemaining <= 0f)
        {
            _matchTimeRemaining = 0f;
            OnMatchTimeExpired();
        }
    }

    private void OnMatchTimeExpired()
    {
        Debug.Log("Match time expired!");
        // Game mode will handle determining winner
    }

    private void ChangeState(MatchState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        MatchStateData data = new MatchStateData(newState, _matchTimeRemaining);
        OnMatchStateChanged?.Invoke(data);

        // Update UI
        if (_matchUI != null)
        {
            _matchUI.OnMatchStateChanged(data);
        }
    }

    private void FinishMatch()
    {
        ChangeState(MatchState.Finished);
        Debug.Log("Match finished!");
    }

    /// <summary>
    /// Restarts the match.
    /// </summary>
    public void RestartMatch()
    {
        _matchTimeRemaining = _matchDuration;
        _startCountdownRemaining = _startCountdown;
        ChangeState(MatchState.Waiting);

        if (_autoStartMatch)
        {
            StartMatch();
        }
    }
}
