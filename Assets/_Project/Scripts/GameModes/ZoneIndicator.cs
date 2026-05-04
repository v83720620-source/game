using UnityEngine;

/// <summary>
/// Visual indicator for capture zone.
/// Changes color based on zone state and controlling team.
/// </summary>
public class ZoneIndicator : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color _neutralColor = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private Color _team1Color = new Color(0f, 0.5f, 1f, 0.6f);
    [SerializeField] private Color _team2Color = new Color(1f, 0.25f, 0.25f, 0.6f);
    [SerializeField] private Color _contestedColor = new Color(1f, 1f, 0f, 0.6f);
    
    [Header("Animation")]
    [SerializeField] private float _pulseSpeed = 1f;
    [SerializeField] private float _pulseIntensity = 0.2f;
    [SerializeField] private bool _enablePulse = true;
    
    [Header("References")]
    [SerializeField] private Renderer _renderer;
    
    private Material _material;
    private Color _targetColor;
    private float _pulseTime;
    
    private void Awake()
    {
        // Auto-find renderer
        if (_renderer == null)
        {
            _renderer = GetComponentInChildren<Renderer>();
        }
        
        // Create material instance
        if (_renderer != null)
        {
            _material = _renderer.material; // Creates instance
            _targetColor = _neutralColor;
        }
    }
    
    private void Update()
    {
        if (_material == null) return;
        
        // Pulse effect
        if (_enablePulse)
        {
            _pulseTime += Time.deltaTime * _pulseSpeed;
            float pulse = Mathf.Sin(_pulseTime) * _pulseIntensity;
            
            Color displayColor = _targetColor;
            displayColor.a = Mathf.Clamp01(_targetColor.a + pulse);
            
            _material.color = displayColor;
        }
        else
        {
            _material.color = _targetColor;
        }
    }
    
    /// <summary>
    /// Update visuals based on zone state.
    /// </summary>
    public void UpdateVisuals(CaptureZone.ZoneState state, Team controllingTeam)
    {
        switch (state)
        {
            case CaptureZone.ZoneState.Neutral:
                _targetColor = _neutralColor;
                break;
                
            case CaptureZone.ZoneState.Capturing:
            case CaptureZone.ZoneState.Controlled:
                _targetColor = GetTeamColor(controllingTeam);
                break;
                
            case CaptureZone.ZoneState.Contested:
                _targetColor = _contestedColor;
                break;
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
    
    private void OnDestroy()
    {
        // Clean up material instance
        if (_material != null)
        {
            Destroy(_material);
        }
    }
}
