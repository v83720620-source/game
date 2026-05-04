using UnityEngine;
using TMPro;

namespace FlumpGame.Debugging
{
    /// <summary>
    /// Simple FPS counter for performance monitoring.
    /// Displays current FPS, average FPS, and frame time.
    /// Works with TextMeshProUGUI.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FPSCounter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _updateInterval = 0.5f;
        [SerializeField] private bool _showOnlyInDevelopment = true;
        [SerializeField] private bool _showDetailedInfo = true;

        [Header("Color Coding")]
        [SerializeField] private Color _goodFPSColor = Color.green;
        [SerializeField] private Color _mediumFPSColor = Color.yellow;
        [SerializeField] private Color _badFPSColor = Color.red;
        [SerializeField] private int _goodFPSThreshold = 45;
        [SerializeField] private int _mediumFPSThreshold = 25;

        private TextMeshProUGUI _fpsText;
        private float _accumulatedTime;
        private int _frames;
        private float _currentFPS;
        private float _averageFPS;
        private float _timeLeft;

        private void Awake()
        {
            _fpsText = GetComponent<TextMeshProUGUI>();
            _timeLeft = _updateInterval;
        }

        private void Start()
        {
            // Hide if not in development build
            if (_showOnlyInDevelopment && !UnityEngine.Debug.isDebugBuild && !Application.isEditor)
            {
                gameObject.SetActive(false);
                return;
            }
        }

        private void Update()
        {
            _timeLeft -= Time.deltaTime;
            _accumulatedTime += Time.timeScale / Time.deltaTime;
            _frames++;

            // Update FPS display
            if (_timeLeft <= 0f)
            {
                _currentFPS = _accumulatedTime / _frames;
                _averageFPS = _frames / _updateInterval;

                _timeLeft = _updateInterval;
                _accumulatedTime = 0f;
                _frames = 0;

                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (_fpsText == null) return;

            // Calculate frame time in milliseconds
            float frameTime = (1f / _currentFPS) * 1000f;

            // Build display text
            string displayText;
            if (_showDetailedInfo)
            {
                displayText = $"FPS: {_currentFPS:F1}\n" +
                              $"AVG: {_averageFPS:F1}\n" +
                              $"MS: {frameTime:F1}";
            }
            else
            {
                displayText = $"FPS: {_currentFPS:F0}";
            }

            _fpsText.text = displayText;

            // Color code based on performance
            _fpsText.color = GetFPSColor(_currentFPS);
        }

        private Color GetFPSColor(float fps)
        {
            if (fps >= _goodFPSThreshold)
            {
                return _goodFPSColor;
            }
            else if (fps >= _mediumFPSThreshold)
            {
                return _mediumFPSColor;
            }
            else
            {
                return _badFPSColor;
            }
        }

        /// <summary>
        /// Get current FPS value.
        /// </summary>
        public float GetCurrentFPS()
        {
            return _currentFPS;
        }

        /// <summary>
        /// Get average FPS value.
        /// </summary>
        public float GetAverageFPS()
        {
            return _averageFPS;
        }

        /// <summary>
        /// Toggle FPS counter visibility.
        /// </summary>
        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _updateInterval = Mathf.Max(0.1f, _updateInterval);
            _goodFPSThreshold = Mathf.Max(1, _goodFPSThreshold);
            _mediumFPSThreshold = Mathf.Max(1, _mediumFPSThreshold);
        }
#endif
    }
}
