using UnityEngine;

/// <summary>
/// Hit box component for body parts.
/// Defines which zone this collider belongs to.
/// </summary>
[RequireComponent(typeof(Collider))]
public class HitBox : MonoBehaviour
{
    [Header("Hit Zone Settings")]
    [SerializeField] private HitZoneType _hitZone = HitZoneType.Body;
    
    [Header("References")]
    [SerializeField] private PlayerHealth _playerHealth;
    
    // Properties
    public HitZoneType HitZone => _hitZone;
    public PlayerHealth PlayerHealth => _playerHealth;
    
    private void Awake()
    {
        // Auto-find PlayerHealth if not set
        if (_playerHealth == null)
        {
            _playerHealth = GetComponentInParent<PlayerHealth>();
        }
        
        // Ensure collider is trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"HitBox '{gameObject.name}' collider should be a trigger!");
        }
    }
    
    /// <summary>
    /// Apply damage to this hit zone.
    /// </summary>
    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_playerHealth != null)
        {
            float finalDamage = damageInfo.GetFinalDamage();
            _playerHealth.TakeDamage(finalDamage, damageInfo.Attacker);
            
            // Log for debugging
            Debug.Log($"Hit {_hitZone}: {damageInfo.BaseDamage} x {DamageInfo.GetZoneMultiplier(_hitZone)} = {finalDamage}");
        }
    }
}
