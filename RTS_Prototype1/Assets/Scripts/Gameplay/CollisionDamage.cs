using UnityEngine;

// This component no longer requires a Rigidbody.
// It works using Trigger colliders.
[RequireComponent(typeof(Health))]
public class CollisionDamage : MonoBehaviour
{
    [Tooltip("The amount of damage this ship takes when it rams another ship.")]
    [SerializeField] private float damageOnRam = 50f;

    private Health _ownHealth;

    private void Awake()
    {
        _ownHealth = GetComponent<Health>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // We only care about collisions with Cargo Ships
        if (other.gameObject.GetComponent<CargoShipAI>() != null)
        {
            // Instantly destroy the cargo ship
            Health cargoHealth = other.gameObject.GetComponent<Health>();
            if (cargoHealth != null)
            {
                // We pass 'gameObject' as the attacker
                cargoHealth.TakeDamage(cargoHealth.MaxHealth, gameObject); 
            }

            // Apply ramming damage to ourselves
            if (_ownHealth != null)
            {
                // Pass 'null' as the attacker since this is self-inflicted/environmental
                _ownHealth.TakeDamage(damageOnRam, null); 
            }
        }
    }
}

