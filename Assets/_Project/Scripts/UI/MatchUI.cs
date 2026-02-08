using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays match information: team scores, timer, and match state.
/// </summary>
public class MatchUI : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] private TextMeshProUGUI _team1ScoreText;
    [SerializeField] private TextMeshProUGUI _team2ScoreText;
    [SerializeField] private TextMeshProUGUI _vsText;

    [Header("Timer Display")]
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("State Display")]
    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private GameObject _statePanel;

    [Header("Settings")]
    [SerializeField] private bool _showStateText = true;

    private MatchManager _matchManager;
    private TeamManager _teamManager;

    private void Start()
    {
        _matchManager = MatchManager.Instance;
        if (_matchManager == null)
        {
            Debug.LogError("[MatchUI] MatchManager not found!");
            return;
        }

        _teamManager = _matchManager.TeamManager;
        if (_teamManager == null)
        {
            Debug.LogError("[MatchUI] TeamManager not found!");
            return;
        }

        // Hide state panel by default
        if (_statePanel != null)
        {
            _statePanel.SetActive(_showStateText);
        }

        // Subscribe to events
        _matchManager.OnMatchStateChanged += OnMatchStateChanged;
    }

    private void OnDestroy()
    {
        if (_matchManager != null)
        {
            _matchManager.OnMatchStateChanged -= OnMatchStateChanged;
        }
    }

    private void Update()
    {
        if (_matchManager == null || _teamManager == null) return;

        UpdateScores();
        UpdateTimer();
    }

    private void UpdateScores()
    {
        TeamData team1 = _teamManager.GetTeam(Team.Team1);
        TeamData team2 = _teamManager.GetTeam(Team.Team2);

        if (_team1ScoreText != null)
        {
            // Show score (works for both TDM kills and Hardpoint points)
            _team1ScoreText.text = team1.score.ToString();
            _team1ScoreText.color = team1.teamColor;
        }

        if (_team2ScoreText != null)
        {
            // Show score (works for both TDM kills and Hardpoint points)
            _team2ScoreText.text = team2.score.ToString();
            _team2ScoreText.color = team2.teamColor;
        }

        if (_vsText != null)
        {
            _vsText.text = "VS";
        }
    }

    private void UpdateTimer()
    {
        if (_timerText == null) return;

        float timeRemaining = _matchManager.MatchTimeRemaining;
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Change color when time is low
        if (timeRemaining <= 30f)
        {
            _timerText.color = Color.red;
        }
        else if (timeRemaining <= 60f)
        {
            _timerText.color = Color.yellow;
        }
        else
        {
            _timerText.color = Color.white;
        }
    }

    public void OnMatchStateChanged(MatchStateData stateData)
    {
        if (!_showStateText || _stateText == null) return;

        switch (stateData.State)
        {
            case MatchState.Waiting:
                _stateText.text = "Waiting for players...";
                if (_statePanel != null) _statePanel.SetActive(true);
                break;

            case MatchState.Starting:
                _stateText.text = "Match starting...";
                if (_statePanel != null) _statePanel.SetActive(true);
                break;

            case MatchState.Playing:
                if (_statePanel != null) _statePanel.SetActive(false);
                break;

            case MatchState.Ending:
                _stateText.text = "Match ending...";
                if (_statePanel != null) _statePanel.SetActive(true);
                break;

            case MatchState.Finished:
                _stateText.text = "Match finished";
                if (_statePanel != null) _statePanel.SetActive(true);
                break;
        }
    }
}
