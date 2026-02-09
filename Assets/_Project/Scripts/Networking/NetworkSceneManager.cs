using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlumpGame.Network
{
    /// <summary>
    /// Управляет загрузкой сцен по сети.
    /// </summary>
    public class NetworkSceneManager : MonoBehaviour
    {
        private NetworkManager _networkManager;
        
        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
            
            if (_networkManager != null && _networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
                _networkManager.SceneManager.OnUnloadEventCompleted += OnUnloadEventCompleted;
            }
        }
        
        private void OnDestroy()
        {
            if (_networkManager != null && _networkManager.SceneManager != null)
            {
                _networkManager.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
                _networkManager.SceneManager.OnUnloadEventCompleted -= OnUnloadEventCompleted;
            }
        }
        
        /// <summary>
        /// Загружает сцену для всех подключенных клиентов (только сервер).
        /// </summary>
        public void LoadSceneNetwork(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (_networkManager == null || !_networkManager.IsServer)
            {
                Debug.LogError("[NetworkSceneManager] Only server can load network scenes!");
                return;
            }
            
            Debug.Log($"[NetworkSceneManager] Loading scene: {sceneName}");
            
            var status = _networkManager.SceneManager.LoadScene(sceneName, loadSceneMode);
            
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogError($"[NetworkSceneManager] Failed to load scene: {sceneName}. Status: {status}");
            }
        }
        
        private void OnLoadEventCompleted(
            string sceneName,
            LoadSceneMode loadSceneMode,
            System.Collections.Generic.List<ulong> clientsCompleted,
            System.Collections.Generic.List<ulong> clientsTimedOut)
        {
            Debug.Log($"[NetworkSceneManager] ✅ Scene loaded: {sceneName}");
            Debug.Log($"[NetworkSceneManager] Clients completed: {clientsCompleted.Count}");
            
            if (clientsTimedOut.Count > 0)
            {
                Debug.LogWarning($"[NetworkSceneManager] Clients timed out: {clientsTimedOut.Count}");
            }
        }
        
        private void OnUnloadEventCompleted(
            string sceneName,
            LoadSceneMode loadSceneMode,
            System.Collections.Generic.List<ulong> clientsCompleted,
            System.Collections.Generic.List<ulong> clientsTimedOut)
        {
            Debug.Log($"[NetworkSceneManager] Scene unloaded: {sceneName}");
        }
    }
}
