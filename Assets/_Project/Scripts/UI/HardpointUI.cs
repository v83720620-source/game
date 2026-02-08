using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI for Hardpoint game mode.
/// Displays zone status, timer, and capture progress.
/// </summary>
public class HardpointUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _zoneStatusText;
    [SerializeField] private TextMeshProUGUI _zoneTimerText;
    [SerializeField] private Slider _captureProgress;
    
    [Header("Colors")]
    [SerializeField] private Color _neutralColor = Color.white;
    [SerializeField] private Color _team1Color = Color.blue;
    [SerializeField] private Color _team2Color = Color.red;
    [SerializeField] private Color _contestedColor = Color.yellow;
    
    private HardpointGameMode _hardpointMode;
    private CaptureZone _currentZone;
    
    private void Start()
    {
        // Find Hardpoint mode
        _hardpointMode = FindAnyObjectByType<HardpointGameMode>();
        
        if (_hardpointMode == null)
        {
            Debug.LogError("[HardpointUI] HardpointGameMode not found!");
            gameObject.SetActive(false);
            return;
        }
    }
    
    private void Update()
    {
        if (_hardpointMode == null) return;
        
        // Update current zone reference
        if (_currentZone != _hardpointMode.CurrentZone)
        {
            UnsubscribeFromZone();
            _currentZone = _hardpointMode.CurrentZone;
            SubscribeToZone();
        }
        
        UpdateUI();
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromZone();
    }
    
    private void SubscribeToZone()
    {
        if (_currentZone != null)
        {
            _currentZone.OnStateChanged += OnZoneStateChanged;
            _currentZone.OnProgressChanged += OnZoneProgressChanged;
        }
    }
    
    private void UnsubscribeFromZone()
    {
        if (_currentZone != null)
        {
            _currentZone.OnStateChanged -= OnZoneStateChanged;
            _currentZone.OnProgressChanged -= OnZoneProgressChanged;
        }
    }
    
    private void UpdateUI()
    {
        if (_currentZone == null) return;
        
        // Update status text
        if (_zoneStatusText != null)
        {
            string statusText = GetStatusText(_currentZone.CurrentState, _currentZone.ControllingTeam);
            _zoneStatusText.text = statusText;
            _zoneStatusText.color = GetStatusColor(_currentZone.CurrentState, _currentZone.ControllingTeam);
        }
        
        // Update timer
        if (_zoneTimerText != null)
        {
            int seconds = Mathf.CeilToInt(_hardpointMode.ZoneTimeRemaining);
            _zoneTimerText.text = $"Next Zone: {seconds}s";
        }
    }
    
    private string GetStatusText(CaptureZone.ZoneState state, Team controllingTeam)
    {
        switch (state)
        {
            case CaptureZone.ZoneState.Neutral:
                return "NEUTRAL";
                
            case CaptureZone.ZoneState.Capturing:
                return $"{GetTeamName(controllingTeam)} CAPTURING...";
                
            case CaptureZone.ZoneState.Controlled:
                return $"{GetTeamName(controllingTeam)} CONTROL";
                
            case CaptureZone.ZoneState.Contested:
                return "CONTESTED!";
                
            default:
                return "";
        }
    }
    
    private Color GetStatusColor(CaptureZone.ZoneState state, Team controllingTeam)
    {
        switch (state)
        {
            case CaptureZone.ZoneState.Neutral:
                return _neutralColor;
                
            case CaptureZone.ZoneState.Capturing:
            case CaptureZone.ZoneState.Controlled:
                return GetTeamColor(controllingTeam);
                
            case CaptureZone.ZoneState.Contested:
                return _contestedColor;
                
            default:
                return _neutralColor;
        }
    }
    
    private Color GetTeamColor(Team team)
    {
        switch (team)
        {
            case Team.Team1:
                return _team1Color;
            case Team.Team2:
                return _team2Color;
            default:
                return _neutralColor;
        }
    }
    
    private string GetTeamName(Team team)
    {
        switch (team)
        {
            case Team.Team1:
                return "BLUE";
            case Team.Team2:
                return "RED";
            default:
                return "NEUTRAL";
        }
    }
    
    private void OnZoneStateChanged(CaptureZone.ZoneState newState)
    {
        // Update immediately when state changes
        UpdateUI();
    }
    
    private void OnZoneProgressChanged(float progress)
    {
        if (_captureProgress != null)
        {
            _captureProgress.value = progress;
            
            // Update fill color
            if (_captureProgress.fillRect != null && _currentZone != null)
            {
                Image fillImage = _captureProgress.fillRect.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = GetStatusColor(_currentZone.CurrentState, _currentZone.ControllingTeam);
                }
            }
        }
    }
}
