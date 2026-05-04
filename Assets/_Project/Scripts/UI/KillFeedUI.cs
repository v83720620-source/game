using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays a feed of recent kills in the match.
/// </summary>
public class KillFeedUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxEntries = 5;
    [SerializeField] private float _entryLifetime = 5f;

    [Header("References")]
    [SerializeField] private Transform _container;
    [SerializeField] private GameObject _entryPrefab;

    private Queue<GameObject> _activeEntries = new Queue<GameObject>();

    private void Start()
    {
        // Create entry prefab if not assigned
        if (_entryPrefab == null)
        {
            CreateDefaultEntryPrefab();
        }
    }

    /// <summary>
    /// Adds a new kill entry to the feed.
    /// </summary>
    public void AddKillEntry(string killerName, string victimName, Team killerTeam, Team victimTeam)
    {
        // Create entry
        GameObject entry = Instantiate(_entryPrefab, _container);
        TextMeshProUGUI text = entry.GetComponent<TextMeshProUGUI>();

        if (text != null)
        {
            // Format: "[Killer] killed [Victim]"
            text.text = $"{killerName} killed {victimName}";

            // Color based on killer team
            if (killerTeam == Team.Team1)
            {
                text.color = Color.blue;
            }
            else if (killerTeam == Team.Team2)
            {
                text.color = Color.red;
            }
            else
            {
                text.color = Color.white;
            }
        }

        // Add to queue
        _activeEntries.Enqueue(entry);

        // Remove old entries if exceeded max
        while (_activeEntries.Count > _maxEntries)
        {
            GameObject oldEntry = _activeEntries.Dequeue();
            Destroy(oldEntry);
        }

        // Auto-remove after lifetime
        StartCoroutine(RemoveEntryAfterDelay(entry, _entryLifetime));
    }

    private IEnumerator RemoveEntryAfterDelay(GameObject entry, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (entry != null)
        {
            // Fade out animation (optional)
            CanvasGroup canvasGroup = entry.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                float fadeDuration = 0.5f;
                float elapsed = 0f;

                while (elapsed < fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                    yield return null;
                }
            }

            Destroy(entry);
        }
    }

    private void CreateDefaultEntryPrefab()
    {
        // Create a simple TextMeshProUGUI prefab
        GameObject prefab = new GameObject("KillFeedEntry");
        TextMeshProUGUI text = prefab.AddComponent<TextMeshProUGUI>();
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Left;

        CanvasGroup canvasGroup = prefab.AddComponent<CanvasGroup>();

        RectTransform rt = prefab.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 30);

        _entryPrefab = prefab;
        Debug.LogWarning("[KillFeedUI] Created default entry prefab. Assign a custom prefab for better visuals!");
    }
}
