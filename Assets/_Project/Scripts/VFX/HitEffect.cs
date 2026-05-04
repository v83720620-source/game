using System.Collections;
using UnityEngine;

namespace FlumpGame.VFX
{
    /// <summary>
    /// Manages hit effects (sparks, dust, etc.) when projectiles hit surfaces.
    /// Supports different surface types with appropriate visual effects.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class HitEffect : MonoBehaviour
    {
        [Header("Effect Settings")]
        [SerializeField] private float _lifetime = 2f;
        [SerializeField] private bool _autoDestroy = true;

        [Header("Surface Types")]
        [SerializeField] private SurfaceType _surfaceType = SurfaceType.Metal;

        private ParticleSystem _particleSystem;
        private float _spawnTime;

        public enum SurfaceType
        {
            Metal,      // Sparks
            Concrete,   // Dust
            Wood,       // Splinters
            Dirt,       // Dust cloud
            Water       // Splash
        }

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            _spawnTime = Time.time;

            // Play particle effect
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }

            // Auto destroy after lifetime
            if (_autoDestroy)
            {
                StartCoroutine(DestroyAfterLifetime());
            }
        }

        /// <summary>
        /// Spawns a hit effect at the specified position and rotation.
        /// </summary>
        /// <param name="position">World position to spawn effect.</param>
        /// <param name="normal">Surface normal for effect orientation.</param>
        /// <param name="surfaceType">Type of surface hit.</param>
        public void Initialize(Vector3 position, Vector3 normal, SurfaceType surfaceType = SurfaceType.Metal)
        {
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(normal);
            _surfaceType = surfaceType;

            // Adjust effect based on surface type
            ApplySurfaceModifications();
        }

        /// <summary>
        /// Modifies particle system based on surface type.
        /// </summary>
        private void ApplySurfaceModifications()
        {
            if (_particleSystem == null) return;

            var main = _particleSystem.main;

            switch (_surfaceType)
            {
                case SurfaceType.Metal:
                    // Sparks - fast, bright, short-lived
                    main.startSpeed = 3f;
                    main.startLifetime = 0.3f;
                    break;

                case SurfaceType.Concrete:
                    // Dust - slower, larger, longer duration
                    main.startSpeed = 1f;
                    main.startLifetime = 0.5f;
                    break;

                case SurfaceType.Wood:
                    // Splinters - medium speed
                    main.startSpeed = 2f;
                    main.startLifetime = 0.4f;
                    break;

                case SurfaceType.Dirt:
                    // Dust cloud - very slow, large
                    main.startSpeed = 0.5f;
                    main.startLifetime = 0.7f;
                    break;

                case SurfaceType.Water:
                    // Splash - fast upward
                    main.startSpeed = 4f;
                    main.startLifetime = 0.3f;
                    break;
            }
        }

        /// <summary>
        /// Coroutine to destroy the effect after its lifetime.
        /// </summary>
        private IEnumerator DestroyAfterLifetime()
        {
            yield return new WaitForSeconds(_lifetime);

            // If using object pooling, return to pool instead
            if (VFXManager.Instance != null && VFXManager.Instance.UseObjectPooling)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Stops the effect immediately.
        /// </summary>
        public void Stop()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Stop();
            }

            StopAllCoroutines();

            if (_autoDestroy)
            {
                StartCoroutine(DestroyAfterLifetime());
            }
        }

        private void OnDisable()
        {
            // Stop all particles when disabled
            if (_particleSystem != null)
            {
                _particleSystem.Stop();
                _particleSystem.Clear();
            }

            StopAllCoroutines();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _lifetime = Mathf.Max(0.1f, _lifetime);
        }
#endif
    }
}
