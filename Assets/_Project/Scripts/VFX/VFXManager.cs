using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central VFX management system.
/// Handles visual effects with object pooling.
/// </summary>
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }
    
    [Header("Pooling Settings")]
    [SerializeField] private bool _useObjectPooling = true;
    [SerializeField] private int _poolSize = 20;
    
    [Header("Effect Prefabs (Optional)")]
    [SerializeField] private GameObject[] _prefabs;
    
    private Dictionary<string, Queue<GameObject>> _effectPools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> _prefabLookup = new Dictionary<string, GameObject>();
    
    // Properties
    public bool UseObjectPooling => _useObjectPooling;
    
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Initialize pools
        if (_useObjectPooling)
        {
            InitializePools();
        }
    }
    
    private void InitializePools()
    {
        if (_prefabs == null) return;
        
        foreach (GameObject prefab in _prefabs)
        {
            if (prefab == null) continue;
            
            string prefabName = prefab.name;
            _prefabLookup[prefabName] = prefab;
            _effectPools[prefabName] = new Queue<GameObject>();
            
            // Pre-instantiate pool objects
            for (int i = 0; i < _poolSize / _prefabs.Length; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                _effectPools[prefabName].Enqueue(obj);
            }
        }
    }
    
    /// <summary>
    /// Spawn an effect at a position.
    /// </summary>
    public GameObject SpawnEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        if (_useObjectPooling && _effectPools.ContainsKey(effectName))
        {
            return GetPooledEffect(effectName, position, rotation);
        }
        else if (_prefabLookup.ContainsKey(effectName))
        {
            return Instantiate(_prefabLookup[effectName], position, rotation);
        }
        
        Debug.LogWarning($"[VFXManager] Effect '{effectName}' not found!");
        return null;
    }
    
    /// <summary>
    /// Spawn effect at position with surface normal.
    /// </summary>
    public GameObject SpawnEffect(string effectName, Vector3 position, Vector3 normal)
    {
        Quaternion rotation = Quaternion.LookRotation(normal);
        return SpawnEffect(effectName, position, rotation);
    }
    
    private GameObject GetPooledEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        Queue<GameObject> pool = _effectPools[effectName];
        
        // Get from pool
        GameObject effect = null;
        
        // Find available object
        int attempts = 0;
        while (pool.Count > 0 && attempts < pool.Count)
        {
            effect = pool.Dequeue();
            
            if (effect != null && !effect.activeSelf)
            {
                break;
            }
            
            // Re-queue if still active
            if (effect != null && effect.activeSelf)
            {
                pool.Enqueue(effect);
                effect = null;
            }
            
            attempts++;
        }
        
        // Create new if none available
        if (effect == null)
        {
            effect = Instantiate(_prefabLookup[effectName], transform);
        }
        
        // Setup effect
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.SetActive(true);
        
        // Return to pool after particle system finishes
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            StartCoroutine(ReturnToPoolAfterParticle(effect, effectName, ps.main.duration + ps.main.startLifetime.constantMax));
        }
        
        return effect;
    }
    
    private System.Collections.IEnumerator ReturnToPoolAfterParticle(GameObject effect, string effectName, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (effect != null)
        {
            effect.SetActive(false);
            
            if (_effectPools.ContainsKey(effectName))
            {
                _effectPools[effectName].Enqueue(effect);
            }
        }
    }
    
    /// <summary>
    /// Spawn muzzle flash.
    /// </summary>
    public void SpawnMuzzleFlash(Vector3 position, Quaternion rotation)
    {
        SpawnEffect("MuzzleFlash", position, rotation);
    }
    
    /// <summary>
    /// Spawn hit effect based on surface type.
    /// </summary>
    public void SpawnHitEffect(Vector3 position, Vector3 normal, string surfaceType = "default")
    {
        string effectName = surfaceType switch
        {
            "metal" => "HitSparkEffect",
            "concrete" => "HitDustEffect",
            _ => "HitSparkEffect"
        };
        
        SpawnEffect(effectName, position, normal);
    }
    
    /// <summary>
    /// Spawn bullet hole decal.
    /// </summary>
    public void SpawnBulletHole(Vector3 position, Vector3 normal)
    {
        SpawnEffect("BulletHole", position, normal);
    }
}
