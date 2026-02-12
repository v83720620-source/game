using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlumpGame.Network
{
    /// <summary>
    /// Система очереди matchmaking.
    /// Управляет поиском игроков и созданием матчей.
    /// </summary>
    public class QueueSystem
    {
        // Таймауты
        private const float BOT_FILL_TIMEOUT = 15f;  // Добавить ботов через 15s
        private const float MATCH_START_TIMEOUT = 60f; // Принудительный старт через 60s
        
        // Состояние
        public enum QueueState
        {
            Idle,           // Не в очереди
            Searching,      // Ищем игроков
            BotFilling,     // Добавляем ботов
            Starting,       // Запускаем матч
            InMatch         // В матче
        }
        
        private QueueState _currentState = QueueState.Idle;
        private float _queueStartTime;
        private int _playersNeeded;
        private int _playersFound;
        
        // События
        public event Action<QueueState> OnStateChanged;
        public event Action<int, int> OnPlayersChanged; // current, needed
        public event Action OnMatchReady;
        
        // Properties
        public QueueState CurrentState => _currentState;
        public float TimeInQueue => _currentState != QueueState.Idle ? Time.time - _queueStartTime : 0f;
        public int PlayersFound => _playersFound;
        public int PlayersNeeded => _playersNeeded;
        
        /// <summary>
        /// Начать поиск игроков.
        /// </summary>
        public void StartSearch(int playersNeeded)
        {
            _playersNeeded = playersNeeded;
            _playersFound = 1; // Локальный игрок
            _queueStartTime = Time.time;
            
            SetState(QueueState.Searching);
            
            OnPlayersChanged?.Invoke(_playersFound, _playersNeeded);
            
            Debug.Log($"[QueueSystem] Started search for {playersNeeded} players");
        }
        
        /// <summary>
        /// Остановить поиск.
        /// </summary>
        public void StopSearch()
        {
            SetState(QueueState.Idle);
            _playersFound = 0;
            
            Debug.Log("[QueueSystem] Search stopped");
        }
        
        /// <summary>
        /// Обновить логику очереди (вызывать из Update).
        /// </summary>
        public void UpdateQueue()
        {
            if (_currentState == QueueState.Idle || _currentState == QueueState.InMatch)
                return;
            
            float timeInQueue = TimeInQueue;
            
            // Проверяем timeout для добавления ботов
            if (_currentState == QueueState.Searching && timeInQueue >= BOT_FILL_TIMEOUT)
            {
                Debug.Log($"[QueueSystem] Bot fill timeout ({BOT_FILL_TIMEOUT}s) - adding bots");
                SetState(QueueState.BotFilling);
                FillWithBots();
            }
            
            // Проверяем timeout для принудительного старта
            if (timeInQueue >= MATCH_START_TIMEOUT)
            {
                Debug.Log($"[QueueSystem] Force start timeout ({MATCH_START_TIMEOUT}s) - starting match");
                StartMatch();
            }
        }
        
        /// <summary>
        /// Добавить игрока в очередь (для multiplayer).
        /// </summary>
        public void AddPlayer()
        {
            _playersFound++;
            OnPlayersChanged?.Invoke(_playersFound, _playersNeeded);
            
            Debug.Log($"[QueueSystem] Player joined: {_playersFound}/{_playersNeeded}");
            
            // Проверяем готовность матча
            if (_playersFound >= _playersNeeded)
            {
                Debug.Log("[QueueSystem] Match full! Starting...");
                StartMatch();
            }
        }
        
        /// <summary>
        /// Удалить игрока из очереди.
        /// </summary>
        public void RemovePlayer()
        {
            _playersFound = Mathf.Max(1, _playersFound - 1); // Минимум локальный игрок
            OnPlayersChanged?.Invoke(_playersFound, _playersNeeded);
            
            Debug.Log($"[QueueSystem] Player left: {_playersFound}/{_playersNeeded}");
        }
        
        private void FillWithBots()
        {
            int botsNeeded = _playersNeeded - _playersFound;
            
            Debug.Log($"[QueueSystem] Adding {botsNeeded} bots to fill match");
            
            // Имитируем добавление ботов
            _playersFound = _playersNeeded;
            OnPlayersChanged?.Invoke(_playersFound, _playersNeeded);
            
            // Запускаем матч
            StartMatch();
        }
        
        private void StartMatch()
        {
            if (_playersFound == 0)
            {
                Debug.LogWarning("[QueueSystem] Cannot start match - no players!");
                return;
            }
            
            SetState(QueueState.Starting);
            
            Debug.Log($"[QueueSystem] Starting match with {_playersFound}/{_playersNeeded} players");
            
            // Уведомляем о готовности матча
            OnMatchReady?.Invoke();
            
            SetState(QueueState.InMatch);
        }
        
        /// <summary>
        /// Сбросить очередь (вызывается когда матч закончился).
        /// </summary>
        public void Reset()
        {
            SetState(QueueState.Idle);
            _playersFound = 0;
            _playersNeeded = 0;
            
            Debug.Log("[QueueSystem] Queue reset");
        }
        
        private void SetState(QueueState newState)
        {
            if (_currentState == newState)
                return;
            
            Debug.Log($"[QueueSystem] State: {_currentState} → {newState}");
            _currentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
