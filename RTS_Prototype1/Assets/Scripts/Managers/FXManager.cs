using UnityEngine;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance { get; private set; }

    [SerializeField] private GameObject explosionFXPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("FXManager: Duplicate FXManager found, destroying this one.", this);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Plays an explosion effect at a given position.
    /// </summary>
    /// <param name="position">The world position to spawn the effect.</param>
    public void PlayExplosionFX(Vector3 position)
    {
        if (explosionFXPrefab != null)
        {
            Instantiate(explosionFXPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("FXManager: explosionFXPrefab is not assigned, cannot play effect.", this);
        }
    }
}
