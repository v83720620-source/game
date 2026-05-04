using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central audio management system.
/// Handles all game sounds with volume control and pooling.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)] private float _masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float _sfxVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float _musicVolume = 0.5f;
    
    [Header("Audio Source Pooling")]
    [SerializeField] private int _maxSimultaneousSounds = 10;
    
    [Header("Sound Effects (Optional - can be empty)")]
    [SerializeField] private AudioClip[] _fireSounds;
    [SerializeField] private AudioClip[] _reloadSounds;
    [SerializeField] private AudioClip[] _hitSounds;
    [SerializeField] private AudioClip[] _footstepSounds;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _respawnSound;
    [SerializeField] private AudioClip _victorySounds;
    [SerializeField] private AudioClip _defeatSound;
    
    private List<AudioSource> _audioSourcePool = new List<AudioSource>();
    private Queue<AudioSource> _availableSources = new Queue<AudioSource>();
    
    // Properties
    public float MasterVolume => _masterVolume;
    public float SFXVolume => _sfxVolume;
    public float MusicVolume => _musicVolume;
    
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Create audio source pool
        InitializeAudioSourcePool();
    }
    
    private void InitializeAudioSourcePool()
    {
        for (int i = 0; i < _maxSimultaneousSounds; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f; // 2D sound by default
            
            _audioSourcePool.Add(source);
            _availableSources.Enqueue(source);
        }
    }
    
    /// <summary>
    /// Play a sound effect at a specific position.
    /// </summary>
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, bool is3D = true)
    {
        if (clip == null) return;
        
        AudioSource source = GetAvailableAudioSource();
        if (source == null) return;
        
        source.clip = clip;
        source.volume = volume * _sfxVolume * _masterVolume;
        source.pitch = pitch;
        source.spatialBlend = is3D ? 1f : 0f; // 1 = 3D, 0 = 2D
        
        if (is3D)
        {
            source.transform.position = position;
            source.minDistance = 1f;
            source.maxDistance = 50f;
        }
        
        source.Play();
        
        // Return to pool after clip finishes
        StartCoroutine(ReturnToPoolAfterPlay(source, clip.length / pitch));
    }
    
    /// <summary>
    /// Play a random sound from an array.
    /// </summary>
    public void PlayRandomSound(AudioClip[] clips, Vector3 position, float volume = 1f, float pitch = 1f, bool is3D = true)
    {
        if (clips == null || clips.Length == 0) return;
        
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        PlaySound(randomClip, position, volume, pitch, is3D);
    }
    
    /// <summary>
    /// Play 2D UI sound.
    /// </summary>
    public void PlayUISound(AudioClip clip, float volume = 1f)
    {
        PlaySound(clip, Vector3.zero, volume, 1f, false);
    }
    
    /// <summary>
    /// Play fire sound.
    /// </summary>
    public void PlayFireSound(Vector3 position)
    {
        PlayRandomSound(_fireSounds, position, 0.8f, Random.Range(0.95f, 1.05f));
    }
    
    /// <summary>
    /// Play reload sound.
    /// </summary>
    public void PlayReloadSound(Vector3 position)
    {
        PlayRandomSound(_reloadSounds, position, 0.6f, 1f);
    }
    
    /// <summary>
    /// Play hit sound.
    /// </summary>
    public void PlayHitSound(Vector3 position)
    {
        PlayRandomSound(_hitSounds, position, 0.5f, Random.Range(0.9f, 1.1f));
    }
    
    /// <summary>
    /// Play footstep sound.
    /// </summary>
    public void PlayFootstepSound(Vector3 position)
    {
        PlayRandomSound(_footstepSounds, position, 0.3f, Random.Range(0.95f, 1.05f));
    }
    
    private AudioSource GetAvailableAudioSource()
    {
        // Try to get from available queue
        while (_availableSources.Count > 0)
        {
            AudioSource source = _availableSources.Dequeue();
            if (!source.isPlaying)
            {
                return source;
            }
        }
        
        // All sources are playing, find one that's almost finished
        AudioSource leastTimeLeft = null;
        float minTimeLeft = float.MaxValue;
        
        foreach (AudioSource source in _audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
            
            float timeLeft = source.clip.length - source.time;
            if (timeLeft < minTimeLeft)
            {
                minTimeLeft = timeLeft;
                leastTimeLeft = source;
            }
        }
        
        return leastTimeLeft;
    }
    
    private System.Collections.IEnumerator ReturnToPoolAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (!source.isPlaying)
        {
            _availableSources.Enqueue(source);
        }
    }
    
    /// <summary>
    /// Set master volume.
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
    }
    
    /// <summary>
    /// Set SFX volume.
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
    }
    
    /// <summary>
    /// Set music volume.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
    }
}
