using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip explosionSound;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {

            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Plays an explosion sound at a given position.
    /// </summary>
    /// <param name="position">The world position to play the sound at.</param>
    public void PlayExplosionSound(Vector3 position)
    {
        if (explosionSound != null)
        {
            // Create a temporary GameObject to host the AudioSource
            GameObject soundObject = new GameObject("TempAudio");
            soundObject.transform.position = position;

            // Add and configure the AudioSource
            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.clip = explosionSound;
            source.spatialBlend = 1.0f; // 3D sound
            source.Play();

            // Destroy the temporary object after the clip has finished playing
            Destroy(soundObject, source.clip.length);
        }
        else
        {

        }
    }
}
