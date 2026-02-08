using System;
using UnityEngine;

/// <summary>
/// Player health system.
/// Manages health, damage, and death.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth = 100f;
    
    [Header("Regeneration")]
    [SerializeField] private bool _autoRegenerate = false;
    [SerializeField] private float _regenerationRate = 5f;
    [SerializeField] private float _regenerationDelay = 3f;
    
    // State
    private bool _isDead;
    private float _lastDamageTime;
    private GameObject _lastAttacker;
    
    // Events
    public event Action OnHealthChanged;
    public event Action<float, GameObject> OnDamageReceived;
    public event Action<GameObject> OnDeath; // Passes killer
    public event Action OnRespawn;
    
    // Properties
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDead => _isDead;
    public float HealthPercentage => _maxHealth > 0 ? _currentHealth / _maxHealth : 0f;
    
    private void Start()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
    }
    
    private void Update()
    {
        // Auto regeneration
        if (_autoRegenerate && !_isDead)
        {
            if (Time.time - _lastDamageTime > _regenerationDelay)
            {
                if (_currentHealth < _maxHealth)
                {
                    Heal(_regenerationRate * Time.deltaTime);
                }
            }
        }
    }
    
    /// <summary>
    /// Apply damage to player.
    /// </summary>
    public void TakeDamage(float damage, GameObject attacker = null)
    {
        if (_isDead || damage <= 0)
            return;
        
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        _lastDamageTime = Time.time;
        _lastAttacker = attacker; // Track who damaged us
        
        // Trigger events
        OnDamageReceived?.Invoke(damage, attacker);
        OnHealthChanged?.Invoke();
        
        // Check death
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Heal player.
    /// </summary>
    public void Heal(float amount)
    {
        if (_isDead || amount <= 0)
            return;
        
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
        
        OnHealthChanged?.Invoke();
    }
    
    /// <summary>
    /// Set health to specific value.
    /// </summary>
    public void SetHealth(float health)
    {
        _currentHealth = Mathf.Clamp(health, 0, _maxHealth);
        OnHealthChanged?.Invoke();
        
        if (_currentHealth <= 0 && !_isDead)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Kill player instantly.
    /// </summary>
    public void Die()
    {
        if (_isDead)
            return;
        
        _isDead = true;
        _currentHealth = 0;
        
        OnDeath?.Invoke(_lastAttacker);
        OnHealthChanged?.Invoke();
        
        // Notify MatchManager
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.RegisterKill(_lastAttacker, gameObject);
        }
        
        Debug.Log($"{gameObject.name} died!");
        
        // Clear last attacker
        _lastAttacker = null;
    }
    
    /// <summary>
    /// Respawn player with full health.
    /// </summary>
    public void Respawn()
    {
        _isDead = false;
        _currentHealth = _maxHealth;
        _lastDamageTime = Time.time;
        
        OnRespawn?.Invoke();
        OnHealthChanged?.Invoke();
        
        Debug.Log("Player respawned!");
    }
    
    /// <summary>
    /// Set max health and optionally heal to full.
    /// </summary>
    public void SetMaxHealth(float maxHealth, bool healToFull = false)
    {
        _maxHealth = Mathf.Max(maxHealth, 1f);
        
        if (healToFull)
        {
            _currentHealth = _maxHealth;
        }
        else
        {
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
        }
        
        OnHealthChanged?.Invoke();
    }
}
