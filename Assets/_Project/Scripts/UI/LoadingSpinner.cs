using UnityEngine;

/// <summary>
/// Simple loading spinner rotation animation.
/// Attach to any UI Image to make it rotate.
/// </summary>
public class LoadingSpinner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _rotationSpeed = 180f; // degrees per second
    
    private void Update()
    {
        // Rotate clockwise (negative Z)
        transform.Rotate(0, 0, -_rotationSpeed * Time.deltaTime);
    }
}
