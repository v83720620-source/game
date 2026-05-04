using UnityEngine;

/// <summary>
/// Hit zone types for damage multipliers.
/// Based on Flump game design document.
/// </summary>
public enum HitZoneType
{
    Head,   // x2.0 damage
    Body,   // x1.0 damage
    Limbs   // x0.75 damage
}

/// <summary>
/// Damage information structure.
/// Contains all data about a damage event.
/// </summary>
public struct DamageInfo
{
    public float BaseDamage;
    public HitZoneType HitZone;
    public Vector3 HitPoint;
    public Vector3 HitDirection;
    public GameObject Attacker;
    public GameObject Victim;
    
    public DamageInfo(float baseDamage, HitZoneType hitZone, Vector3 hitPoint, Vector3 hitDirection, GameObject attacker, GameObject victim = null)
    {
        BaseDamage = baseDamage;
        HitZone = hitZone;
        HitPoint = hitPoint;
        HitDirection = hitDirection;
        Attacker = attacker;
        Victim = victim;
    }
    
    /// <summary>
    /// Calculate final damage with zone multiplier.
    /// </summary>
    public float GetFinalDamage()
    {
        float multiplier = GetZoneMultiplier(HitZone);
        return BaseDamage * multiplier;
    }
    
    /// <summary>
    /// Get damage multiplier for hit zone.
    /// </summary>
    public static float GetZoneMultiplier(HitZoneType zone)
    {
        switch (zone)
        {
            case HitZoneType.Head:
                return 2.0f;
            case HitZoneType.Body:
                return 1.0f;
            case HitZoneType.Limbs:
                return 0.75f;
            default:
                return 1.0f;
        }
    }
}
