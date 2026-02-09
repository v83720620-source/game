using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace FlumpGame.UI
{
    /// <summary>
    /// Управляет загрузкой сцен в игре.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _instance;
        
        public static SceneLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("SceneLoader");
                    _instance = go.AddComponent<SceneLoader>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        /// <summary>
        /// Загружает сцену по имени.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Debug.Log($"[SceneLoader] Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
        
        /// <summary>
        /// Загружает сцену по индексу.
        /// </summary>
        public void LoadScene(int sceneIndex)
        {
            Debug.Log($"[SceneLoader] Loading scene by index: {sceneIndex}");
            SceneManager.LoadScene(sceneIndex);
        }
        
        /// <summary>
        /// Загружает сцену асинхронно с прогрессом.
        /// </summary>
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }
        
        private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            Debug.Log($"[SceneLoader] Loading scene async: {sceneName}");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncLoad.isDone)
            {
                float progress = asyncLoad.progress;
                // TODO: Обновить UI loading bar
                yield return null;
            }
            
            Debug.Log($"[SceneLoader] Scene loaded: {sceneName}");
        }
        
        /// <summary>
        /// Перезагружает текущую сцену.
        /// </summary>
        public void ReloadCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            Debug.Log($"[SceneLoader] Reloading scene: {currentScene.name}");
            SceneManager.LoadScene(currentScene.buildIndex);
        }
        
        /// <summary>
        /// Выход из игры.
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("[SceneLoader] Quitting game...");
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
