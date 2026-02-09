using Unity.Netcode;
using UnityEngine;

namespace FlumpGame.Network
{
    /// <summary>
    /// Кастомный Network Manager для игры.
    /// Управляет подключением, авторизацией и сессиями.
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public class CustomNetworkManager : MonoBehaviour
    {
        private static CustomNetworkManager _instance;
        public static CustomNetworkManager Instance => _instance;
        
        private NetworkManager _networkManager;
        
        [Header("Server Settings")]
        [SerializeField] private int _maxConnections = 10;
        
        [Header("Debug")]
        [SerializeField] private bool _startServerOnAwake = false;
        [SerializeField] private bool _startHostOnAwake = false;
        [SerializeField] private bool _startClientOnAwake = false;
        
        private void Awake()
        {
            // Singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            
            _networkManager = GetComponent<NetworkManager>();
            
            // Подписываемся на события
            _networkManager.OnServerStarted += OnServerStarted;
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
            
            // Connection Approval
            _networkManager.ConnectionApprovalCallback = ApprovalCheck;
        }
        
        private void Start()
        {
            // Автозапуск для тестирования
            if (_startServerOnAwake)
                StartServer();
            else if (_startHostOnAwake)
                StartHost();
            else if (_startClientOnAwake)
                StartClient();
        }
        
        private void OnDestroy()
        {
            if (_networkManager != null)
            {
                _networkManager.OnServerStarted -= OnServerStarted;
                _networkManager.OnClientConnectedCallback -= OnClientConnected;
                _networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            }
            
            if (_instance == this)
                _instance = null;
        }
        
        // ============================================
        // PUBLIC API
        // ============================================
        
        /// <summary>
        /// Запускает сервер.
        /// </summary>
        public bool StartServer()
        {
            Debug.Log("[NetworkManager] Starting server...");
            return _networkManager.StartServer();
        }
        
        /// <summary>
        /// Запускает хост (сервер + клиент).
        /// </summary>
        public bool StartHost()
        {
            Debug.Log("[NetworkManager] Starting host...");
            return _networkManager.StartHost();
        }
        
        /// <summary>
        /// Запускает клиент и подключается к серверу.
        /// </summary>
        public bool StartClient()
        {
            Debug.Log("[NetworkManager] Starting client...");
            return _networkManager.StartClient();
        }
        
        /// <summary>
        /// Отключается от сети.
        /// </summary>
        public void Shutdown()
        {
            Debug.Log("[NetworkManager] Shutting down network...");
            _networkManager.Shutdown();
        }
        
        /// <summary>
        /// Проверяет, является ли текущий клиент хостом.
        /// </summary>
        public bool IsHost()
        {
            return _networkManager != null && _networkManager.IsHost;
        }
        
        /// <summary>
        /// Проверяет, является ли текущий клиент сервером.
        /// </summary>
        public bool IsServer()
        {
            return _networkManager != null && _networkManager.IsServer;
        }
        
        /// <summary>
        /// Проверяет, подключен ли клиент.
        /// </summary>
        public bool IsClient()
        {
            return _networkManager != null && _networkManager.IsClient;
        }
        
        // ============================================
        // CONNECTION APPROVAL
        // ============================================
        
        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            // Проверяем количество подключений
            if (_networkManager.ConnectedClientsIds.Count >= _maxConnections)
            {
                response.Approved = false;
                response.Reason = "Server is full";
                Debug.LogWarning($"[NetworkManager] Connection rejected: Server is full ({_maxConnections}/{_maxConnections})");
                return;
            }
            
            // TODO: Дополнительные проверки:
            // - Версия клиента
            // - Бан лист
            // - Пароль комнаты
            
            // Принимаем подключение
            response.Approved = true;
            response.CreatePlayerObject = true; // Создаём player object автоматически
            
            Debug.Log($"[NetworkManager] Connection approved for client {request.ClientNetworkId}");
        }
        
        // ============================================
        // NETWORK CALLBACKS
        // ============================================
        
        private void OnServerStarted()
        {
            Debug.Log("[NetworkManager] ✅ Server started successfully!");
        }
        
        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"[NetworkManager] ✅ Client connected: {clientId}");
            
            if (_networkManager.IsServer)
            {
                Debug.Log($"[NetworkManager] Total clients connected: {_networkManager.ConnectedClientsIds.Count}");
            }
        }
        
        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log($"[NetworkManager] ❌ Client disconnected: {clientId}");
            
            if (_networkManager.IsServer)
            {
                Debug.Log($"[NetworkManager] Remaining clients: {_networkManager.ConnectedClientsIds.Count}");
                
                // TODO: Handle player leaving
                // - Remove player from match
                // - Notify other players
                // - Spawn bot replacement (Этап 16)
            }
        }
    }
}
