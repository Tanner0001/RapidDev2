using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int creditsOnDeath = 25;
    
    private float _currentHealth;
    private GameObject _lastAttacker;

    public event Action<GameObject> OnDeath;
    public event Action<GameObject, GameObject> OnDamaged; // Victim, Attacker
    public event Action<GameObject> OnRepaired;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => _currentHealth <= 0;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, GameObject attacker)
    {
        if (IsDead) return;

        _currentHealth -= amount;
        _lastAttacker = attacker;
        OnDamaged?.Invoke(gameObject, attacker);

        if (_currentHealth <= 0) // Ensure Die is only called once
        {
             Die();
        }
    }
    
    public void Repair(float amount)
    {
        if (IsDead || _currentHealth >= maxHealth) return;

        _currentHealth += amount;
        if (_currentHealth > maxHealth)
        {
            _currentHealth = maxHealth;
        }

        OnRepaired?.Invoke(gameObject);
    }

    private void Die()
    {
        // If this is a cargo ship, it should just despawn without fanfare.
        if (GetComponent<CargoShipAI>() != null)
        {
            Destroy(gameObject);
            return;
        }
        
        OnDeath?.Invoke(gameObject);

        // Grant credits if the killer was a player
        if (_lastAttacker != null && _lastAttacker.GetComponent<PlayerUnit>() != null)
        {
            if (GameManager.Instance != null && creditsOnDeath > 0)
            {
                GameManager.Instance.AddCredits(creditsOnDeath);
            }
        }
        
        // Notify GameManager if a civilian ship was destroyed by a player
        UnitFaction faction = GetComponent<UnitFaction>();
        if (faction != null && faction.UnitFactionType == Faction.Civilian)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DecreaseNationHealth(10); // Example value
            }
        }

        // Optional: Add explosion effect and sound from managers
        if (FXManager.Instance != null)
        {
            FXManager.Instance.PlayExplosionFX(transform.position);
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayExplosionSound(transform.position);
        }

        // The object that died is responsible for destroying itself.
        Destroy(gameObject);
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        _currentHealth += amount; // Also increase current health
    }

    public void ApplyDifficultyScaling(float multiplier)
    {
        maxHealth *= multiplier;
        _currentHealth = maxHealth;
    }
}

