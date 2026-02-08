using UnityEngine;

/// <summary>
/// Team Deathmatch game mode logic.
/// First team to reach kill limit wins, or team with most kills when time expires.
/// </summary>
[RequireComponent(typeof(MatchManager))]
public class TDMGameMode : MonoBehaviour
{
    [Header("TDM Settings")]
    [SerializeField] private int _killLimit = 50;

    private MatchManager _matchManager;
    private TeamManager _teamManager;

    private void Awake()
    {
        _matchManager = GetComponent<MatchManager>();
    }

    private void Start()
    {
        // Get TeamManager
        _teamManager = _matchManager.TeamManager;

        if (_teamManager == null)
        {
            Debug.LogError("[TDMGameMode] TeamManager not found!");
            return;
        }

        // Subscribe to events
        _matchManager.OnPlayerKilled += OnPlayerKilled;
        _matchManager.OnMatchStateChanged += OnMatchStateChanged;
    }

    private void OnDestroy()
    {
        if (_matchManager != null)
        {
            _matchManager.OnPlayerKilled -= OnPlayerKilled;
            _matchManager.OnMatchStateChanged -= OnMatchStateChanged;
        }
    }

    private void OnPlayerKilled(GameObject killer, GameObject victim, Team killerTeam, Team victimTeam)
    {
        // Only count kills during active match
        if (_matchManager.CurrentState != MatchState.Playing) return;

        // Ignore suicide or teamkills for score (optional)
        if (killerTeam == victimTeam || killerTeam == Team.None) return;

        // Add kill to killer's team
        _teamManager.AddKill(killerTeam);

        // Add death to victim's team
        _teamManager.AddDeath(victimTeam);

        Debug.Log($"[TDM] {killerTeam} kills: {_teamManager.GetTeam(killerTeam).kills}");

        // Check win condition
        CheckWinCondition();
    }

    private void OnMatchStateChanged(MatchStateData stateData)
    {
        if (stateData.State == MatchState.Playing)
        {
            // Reset scores
            _teamManager.ResetScores();
            Debug.Log("[TDM] Match started! First to " + _killLimit + " kills wins!");
        }
    }

    private void Update()
    {
        // Check if time expired
        if (_matchManager.CurrentState == MatchState.Playing && _matchManager.MatchTimeRemaining <= 0f)
        {
            OnTimeExpired();
        }
    }

    private void CheckWinCondition()
    {
        TeamData team1 = _teamManager.GetTeam(Team.Team1);
        TeamData team2 = _teamManager.GetTeam(Team.Team2);

        // Check kill limit
        if (team1.kills >= _killLimit)
        {
            _matchManager.EndMatch(Team.Team1, $"Reached {_killLimit} kills");
        }
        else if (team2.kills >= _killLimit)
        {
            _matchManager.EndMatch(Team.Team2, $"Reached {_killLimit} kills");
        }
    }

    private void OnTimeExpired()
    {
        TeamData team1 = _teamManager.GetTeam(Team.Team1);
        TeamData team2 = _teamManager.GetTeam(Team.Team2);

        // Team with most kills wins
        if (team1.kills > team2.kills)
        {
            _matchManager.EndMatch(Team.Team1, "Most kills when time expired");
        }
        else if (team2.kills > team1.kills)
        {
            _matchManager.EndMatch(Team.Team2, "Most kills when time expired");
        }
        else
        {
            // Draw - can assign to Team.None or pick random winner
            _matchManager.EndMatch(Team.None, "Draw - equal kills");
        }
    }
}
