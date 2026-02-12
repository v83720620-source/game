using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network.Bots
{
    /// <summary>
    /// Обнаруживает AFK (Away From Keyboard) игроков.
    /// Автоматически кикает после timeout.
    /// </summary>
    public class AFKDetector : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _afkTimeout = 60f; // 60 секунд без активности
        [SerializeField] private float _afkWarningTime = 45f; // Предупреждение за 15s до кика
        [SerializeField] private bool _enableAFKKick = true;
        
        // Player activity tracking
        private float _lastActivityTime;
        private bool _warningShown;
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            _lastActivityTime = Time.time;
            _warningShown = false;
        }
        
        private void Update()
        {
            if (!IsOwner) return;
            if (!_enableAFKKick) return;
            
            // Check for any input activity
            if (HasPlayerActivity())
            {
                _lastActivityTime = Time.time;
                _warningShown = false;
            }
            
            float inactiveTime = Time.time - _lastActivityTime;
            
            // Show warning
            if (!_warningShown && inactiveTime >= _afkWarningTime)
            {
                _warningShown = true;
                ShowAFKWarningClientRpc();
            }
            
            // Kick if timeout
            if (inactiveTime >= _afkTimeout)
            {
                Debug.Log($"[AFKDetector] Player AFK for {inactiveTime}s - disconnecting");
                RequestKickServerRpc();
            }
        }
        
        /// <summary>
        /// Check if player has any activity.
        /// </summary>
        private bool HasPlayerActivity()
        {
            // Check for any input
            bool hasInput = 
                Input.anyKey ||
                Input.GetAxis("Mouse X") != 0 ||
                Input.GetAxis("Mouse Y") != 0 ||
                Input.GetMouseButton(0) ||
                Input.GetMouseButton(1);
            
            return hasInput;
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void ShowAFKWarningClientRpc()
        {
            if (!IsOwner) return;
            
            Debug.LogWarning("[AFKDetector] ⚠️ AFK Warning! You will be kicked for inactivity!");
            
            // TODO: Show UI notification
            // "You will be kicked for inactivity in 15 seconds!"
        }
        
        [Rpc(SendTo.Server)]
        private void RequestKickServerRpc()
        {
            if (!IsServer) return;
            
            Debug.Log($"[AFKDetector] Kicking AFK player {OwnerClientId}");
            
            // Disconnect player
            NetworkManager.Singleton.DisconnectClient(OwnerClientId);
            
            // BackfillManager will automatically spawn bot replacement
        }
        
        /// <summary>
        /// Manually reset activity timer.
        /// </summary>
        public void ResetActivityTimer()
        {
            _lastActivityTime = Time.time;
            _warningShown = false;
        }
        
        /// <summary>
        /// Get inactive time.
        /// </summary>
        public float GetInactiveTime()
        {
            return Time.time - _lastActivityTime;
        }
    }
}
