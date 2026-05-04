using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Ammo display UI component.
/// Shows current ammo in magazine and reserve ammo.
/// Supports both Text and TextMeshPro.
/// </summary>
public class AmmoDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text _ammoText;
    [SerializeField] private TextMeshProUGUI _ammoTextTMP;
    
    [Header("Format")]
    [SerializeField] private string _format = "{0} / {1}";
    [SerializeField] private bool _showInfiniteSymbol = true;
    
    [Header("Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _lowAmmoColor = Color.yellow;
    [SerializeField] private Color _emptyColor = Color.red;
    [SerializeField] private float _lowAmmoThreshold = 0.3f;
    
    // State
    private int _currentAmmo;
    private int _maxAmmo;
    private int _reserveAmmo;
    private bool _hasInfiniteAmmo;
    
    private void Awake()
    {
        if (_ammoText == null && _ammoTextTMP == null)
        {
            _ammoText = GetComponent<Text>();
            _ammoTextTMP = GetComponent<TextMeshProUGUI>();
        }
    }
    
    /// <summary>
    /// Update ammo display.
    /// </summary>
    public void SetAmmo(int currentAmmo, int maxAmmo, int reserveAmmo = -1)
    {
        _currentAmmo = currentAmmo;
        _maxAmmo = maxAmmo;
        _reserveAmmo = reserveAmmo;
        _hasInfiniteAmmo = reserveAmmo < 0;
        
        UpdateDisplay();
    }
    
    private void UpdateDisplay()
    {
        if (_ammoText == null && _ammoTextTMP == null)
            return;
        
        // Format text
        string reserveText;
        if (_hasInfiniteAmmo && _showInfiniteSymbol)
        {
            reserveText = "∞";
        }
        else if (_hasInfiniteAmmo)
        {
            reserveText = "999";
        }
        else
        {
            reserveText = _reserveAmmo.ToString();
        }
        
        string displayText = string.Format(_format, _currentAmmo, reserveText);
        
        // Update color
        float ammoRatio = _maxAmmo > 0 ? (float)_currentAmmo / _maxAmmo : 0f;
        Color targetColor;
        
        if (_currentAmmo <= 0)
        {
            targetColor = _emptyColor;
        }
        else if (ammoRatio <= _lowAmmoThreshold)
        {
            targetColor = _lowAmmoColor;
        }
        else
        {
            targetColor = _normalColor;
        }
        
        // Set text and color for both types
        if (_ammoText != null)
        {
            _ammoText.text = displayText;
            _ammoText.color = targetColor;
        }
        
        if (_ammoTextTMP != null)
        {
            _ammoTextTMP.text = displayText;
            _ammoTextTMP.color = targetColor;
        }
    }
    
    /// <summary>
    /// Flash effect when reloading (optional).
    /// </summary>
    public void PlayReloadEffect()
    {
        // TODO: Add reload animation
        UpdateDisplay();
    }
}
