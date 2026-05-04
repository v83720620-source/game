using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays the match end screen with victory/defeat and final scores.
/// </summary>
public class MatchEndUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TextMeshProUGUI _team1ScoreText;
    [SerializeField] private TextMeshProUGUI _team2ScoreText;
    [SerializeField] private TextMeshProUGUI _reasonText;

    [Header("Buttons")]
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _exitButton;

    [Header("Settings")]
    [SerializeField] private Color _victoryColor = Color.green;
    [SerializeField] private Color _defeatColor = Color.red;
    [SerializeField] private Color _drawColor = Color.yellow;

    private void Start()
    {
        // Hide panel by default
        if (_panel != null)
        {
            _panel.SetActive(false);
        }

        // Setup buttons
        if (_playAgainButton != null)
        {
            _playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }

        if (_exitButton != null)
        {
            _exitButton.onClick.AddListener(OnExitClicked);
        }
    }

    /// <summary>
    /// Shows the match end screen.
    /// </summary>
    public void ShowMatchEnd(Team winningTeam, TeamData team1, TeamData team2, string reason)
    {
        if (_panel != null)
        {
            _panel.SetActive(true);
        }

        // Determine if player won (assuming player is Team1)
        bool playerWon = winningTeam == Team.Team1;
        bool isDraw = winningTeam == Team.None;

        // Set result text
        if (_resultText != null)
        {
            if (isDraw)
            {
                _resultText.text = "DRAW!";
                _resultText.color = _drawColor;
            }
            else if (playerWon)
            {
                _resultText.text = "VICTORY!";
                _resultText.color = _victoryColor;
            }
            else
            {
                _resultText.text = "DEFEAT!";
                _resultText.color = _defeatColor;
            }
        }

        // Set scores
        if (_team1ScoreText != null)
        {
            _team1ScoreText.text = $"{team1.teamName}: {team1.score}";
            _team1ScoreText.color = team1.teamColor;
        }

        if (_team2ScoreText != null)
        {
            _team2ScoreText.text = $"{team2.teamName}: {team2.score}";
            _team2ScoreText.color = team2.teamColor;
        }

        // Set reason
        if (_reasonText != null)
        {
            _reasonText.text = reason;
        }

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log($"[MatchEndUI] Showing match end: Winner={winningTeam}, Reason={reason}");
    }

    /// <summary>
    /// Hides the match end screen.
    /// </summary>
    public void HideMatchEnd()
    {
        if (_panel != null)
        {
            _panel.SetActive(false);
        }

        // Lock cursor back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnPlayAgainClicked()
    {
        Debug.Log("[MatchEndUI] Play Again clicked");
        HideMatchEnd();

        // Restart match
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.RestartMatch();
        }
    }

    private void OnExitClicked()
    {
        Debug.Log("[MatchEndUI] Exit clicked");

        // Exit game (or return to menu in full game)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
