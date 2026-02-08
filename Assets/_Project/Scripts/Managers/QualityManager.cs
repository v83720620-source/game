using UnityEngine;

namespace FlumpGame.Managers
{
    /// <summary>
    /// Manages graphics quality settings for optimal performance on different devices.
    /// Supports automatic quality detection and manual quality adjustment.
    /// </summary>
    public class QualityManager : MonoBehaviour
    {
        public static QualityManager Instance { get; private set; }

        [Header("Quality Settings")]
        [SerializeField] private bool _autoDetectQuality = true;
        [SerializeField] private int _defaultQualityLevel = 1; // 0=Low, 1=Medium, 2=High

        [Header("Frame Rate")]
        [SerializeField] private int _targetFrameRateMobile = 60;
        [SerializeField] private int _targetFrameRateStandalone = 144;
        [SerializeField] private bool _enableVSync = false;

        [Header("Battery Saver")]
        [SerializeField] private bool _batterySaverMode = false;
        [SerializeField] private int _batterySaverFrameRate = 30;
        [SerializeField] private int _batterySaverQualityLevel = 0; // Low

        private int _currentQualityLevel;
        private bool _isMobile;

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

            // Detect platform
            _isMobile = Application.isMobilePlatform;

            // Initialize quality
            InitializeQuality();
        }

        private void InitializeQuality()
        {
            // Disable VSync for manual frame rate control
            QualitySettings.vSyncCount = _enableVSync ? 1 : 0;

            // Set target frame rate
            int targetFPS = _isMobile ? _targetFrameRateMobile : _targetFrameRateStandalone;
            Application.targetFrameRate = targetFPS;

            // Auto-detect or set default quality
            if (_autoDetectQuality)
            {
                DetectAndSetQuality();
            }
            else
            {
                SetQualityLevel(_defaultQualityLevel);
            }

            // Apply battery saver if enabled
            if (_batterySaverMode)
            {
                EnableBatterySaver();
            }

            Debug.Log($"[QualityManager] Initialized - Platform: {(_isMobile ? "Mobile" : "PC")}, Quality: {QualitySettings.names[_currentQualityLevel]}, FPS: {Application.targetFrameRate}");
        }

        /// <summary>
        /// Auto-detect device performance and set appropriate quality.
        /// </summary>
        private void DetectAndSetQuality()
        {
            int qualityLevel = 1; // Default to Medium

            if (_isMobile)
            {
                // Mobile device detection
                int systemMemory = SystemInfo.systemMemorySize;
                int processorCount = SystemInfo.processorCount;

                if (systemMemory < 2048 || processorCount < 4)
                {
                    qualityLevel = 0; // Low - weak devices
                }
                else if (systemMemory < 4096 || processorCount < 6)
                {
                    qualityLevel = 1; // Medium - average devices
                }
                else
                {
                    qualityLevel = 2; // High - powerful devices
                }
            }
            else
            {
                // PC - default to High
                qualityLevel = 2;
            }

            SetQualityLevel(qualityLevel);
        }

        /// <summary>
        /// Set quality level manually.
        /// </summary>
        /// <param name="level">0 = Low, 1 = Medium, 2 = High</param>
        public void SetQualityLevel(int level)
        {
            _currentQualityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(_currentQualityLevel, true);

            Debug.Log($"[QualityManager] Quality set to: {QualitySettings.names[_currentQualityLevel]}");
        }

        /// <summary>
        /// Get current quality level.
        /// </summary>
        public int GetCurrentQualityLevel()
        {
            return _currentQualityLevel;
        }

        /// <summary>
        /// Enable battery saver mode (lower quality and FPS).
        /// </summary>
        public void EnableBatterySaver()
        {
            _batterySaverMode = true;
            SetQualityLevel(_batterySaverQualityLevel);
            Application.targetFrameRate = _batterySaverFrameRate;

            Debug.Log($"[QualityManager] Battery Saver enabled - FPS: {_batterySaverFrameRate}");
        }

        /// <summary>
        /// Disable battery saver mode.
        /// </summary>
        public void DisableBatterySaver()
        {
            _batterySaverMode = false;
            int targetFPS = _isMobile ? _targetFrameRateMobile : _targetFrameRateStandalone;
            Application.targetFrameRate = targetFPS;
            SetQualityLevel(_defaultQualityLevel);

            Debug.Log($"[QualityManager] Battery Saver disabled - FPS: {targetFPS}");
        }

        /// <summary>
        /// Toggle battery saver mode.
        /// </summary>
        public void ToggleBatterySaver()
        {
            if (_batterySaverMode)
            {
                DisableBatterySaver();
            }
            else
            {
                EnableBatterySaver();
            }
        }

        /// <summary>
        /// Set target frame rate.
        /// </summary>
        public void SetTargetFrameRate(int fps)
        {
            Application.targetFrameRate = fps;
            Debug.Log($"[QualityManager] Target FPS set to: {fps}");
        }

        /// <summary>
        /// Get device performance info.
        /// </summary>
        public string GetDeviceInfo()
        {
            return $"Device: {SystemInfo.deviceModel}\n" +
                   $"OS: {SystemInfo.operatingSystem}\n" +
                   $"RAM: {SystemInfo.systemMemorySize} MB\n" +
                   $"GPU: {SystemInfo.graphicsDeviceName}\n" +
                   $"CPU: {SystemInfo.processorType} ({SystemInfo.processorCount} cores)\n" +
                   $"Quality: {QualitySettings.names[_currentQualityLevel]}\n" +
                   $"Target FPS: {Application.targetFrameRate}";
        }

#if UNITY_EDITOR
        [ContextMenu("Print Device Info")]
        private void PrintDeviceInfo()
        {
            Debug.Log(GetDeviceInfo());
        }

        [ContextMenu("Set Quality: Low")]
        private void SetLowQuality() => SetQualityLevel(0);

        [ContextMenu("Set Quality: Medium")]
        private void SetMediumQuality() => SetQualityLevel(1);

        [ContextMenu("Set Quality: High")]
        private void SetHighQuality() => SetQualityLevel(2);
#endif
    }
}
