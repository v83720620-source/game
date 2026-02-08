using UnityEngine;

/// <summary>
/// Bullet hole decal that fades out over time.
/// </summary>
public class BulletHole : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _lifetime = 30f;
    [SerializeField] private float _fadeDuration = 5f;
    
    private Renderer _renderer;
    private Material _material;
    private float _elapsedTime;
    private Color _originalColor;
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        
        if (_renderer != null)
        {
            _material = _renderer.material; // Create instance
            _originalColor = _material.color;
        }
    }
    
    private void OnEnable()
    {
        _elapsedTime = 0f;
        
        if (_material != null)
        {
            _material.color = _originalColor;
        }
    }
    
    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        
        // Start fading near the end of lifetime
        if (_elapsedTime >= _lifetime - _fadeDuration)
        {
            float fadeProgress = (_elapsedTime - (_lifetime - _fadeDuration)) / _fadeDuration;
            
            if (_material != null)
            {
                Color color = _originalColor;
                color.a = Mathf.Lerp(_originalColor.a, 0f, fadeProgress);
                _material.color = color;
            }
        }
        
        // Destroy or disable after lifetime
        if (_elapsedTime >= _lifetime)
        {
            // If using object pooling, deactivate; otherwise destroy
            if (VFXManager.Instance != null && VFXManager.Instance.UseObjectPooling)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    
    private void OnDestroy()
    {
        if (_material != null)
        {
            Destroy(_material);
        }
    }
}
