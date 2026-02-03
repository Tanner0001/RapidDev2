using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip selectionSound;
    [SerializeField] private AudioClip moveCommandSound;
    [SerializeField] private AudioClip attackCommandSound;
    [SerializeField] private AudioClip unitBuiltSound;
    [SerializeField] private AudioClip weaponFireSound;
    
    private AudioSource _audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    private void PlayClipAtPoint(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    public void PlayExplosionSound(Vector3 position)
    {
        PlaySound(explosionSound);
    }

    public void PlaySelectionSound()
    {
        PlaySound(selectionSound);
    }

    public void PlayMoveCommandSound()
    {
        PlaySound(moveCommandSound);
    }

    public void PlayAttackCommandSound()
    {
        PlaySound(attackCommandSound);
    }
    
    public void PlayUnitBuiltSound()
    {
        PlaySound(unitBuiltSound);
    }

    public void PlayWeaponFireSound(Vector3 position)
    {
        PlaySound(weaponFireSound);
    }
}
