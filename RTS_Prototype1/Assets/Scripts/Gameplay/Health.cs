using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;

    public event Action<GameObject> OnDeath;
    public event Action<GameObject, GameObject> OnDamaged; // Victim, Attacker

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
        OnDamaged?.Invoke(gameObject, attacker);


        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {

        OnDeath?.Invoke(gameObject);

        // Notify GameManager if a civilian ship was destroyed
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
}
