using UnityEngine;

/// <summary>
/// Weapon data ScriptableObject.
/// Contains all weapon stats and configuration.
/// </summary>
[CreateAssetMenu(fileName = "WeaponData", menuName = "Flump/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName = "Assault Rifle";
    public Sprite weaponIcon;
    
    [Header("Damage")]
    public float baseDamage = 20f;
    public float range = 100f;
    
    [Header("Fire Rate")]
    public float fireRate = 0.1f;           // Time between shots
    public bool isAutomatic = true;          // Auto-fire or semi-auto
    
    [Header("Magazine")]
    public int magazineSize = 30;            // Bullets per magazine
    public int reserveAmmo = 90;             // Extra ammo
    public float reloadTime = 2f;            // Reload duration
    
    [Header("Recoil")]
    public float recoilAmount = 1f;
    public Vector2 recoilPattern = new Vector2(0.3f, 0.8f);
    public float recoilRecoverySpeed = 5f;
    
    [Header("Spread")]
    public float baseSpread = 0.01f;         // Accuracy
    public float maxSpread = 0.1f;
    public float spreadIncreasePerShot = 0.01f;
    public float spreadDecreaseSpeed = 5f;
    
    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
}
