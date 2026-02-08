using UnityEngine;

/// <summary>
/// Muzzle flash effect for weapons.
/// Automatically destroys or disables after flash duration.
/// </summary>
public class MuzzleFlash : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _flashDuration = 0.05f;
    [SerializeField] private bool _autoDestroy = true;
    
    [Header("Components")]
    [SerializeField] private Light _light;
    [SerializeField] private Renderer _renderer;
    
    private float _elapsedTime;
    
    private void Awake()
    {
        // Auto-find components
        if (_light == null)
        {
            _light = GetComponent<Light>();
        }
        
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }
    }
    
    private void OnEnable()
    {
        _elapsedTime = 0f;
    }
    
    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        
        if (_elapsedTime >= _flashDuration)
        {
            // If using object pooling, deactivate; otherwise destroy
            if (VFXManager.Instance != null && VFXManager.Instance.UseObjectPooling)
            {
                gameObject.SetActive(false);
            }
            else if (_autoDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
