using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RepairZone : MonoBehaviour
{
    [Tooltip("The amount of health restored per second to each ship inside the zone.")]
    [SerializeField] private float repairRate = 15f;

    private List<Health> _shipsToRepair = new List<Health>();
    private BoxCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true; // Ensure the collider is a trigger
    }

    private void Update()
    {
        if (_shipsToRepair.Count == 0) return;

        // Repair all ships currently in the zone
        for (int i = _shipsToRepair.Count - 1; i >= 0; i--)
        {
            Health shipHealth = _shipsToRepair[i];
            if (shipHealth == null || shipHealth.IsDead)
            {
                // Remove destroyed or invalid ships from the list
                _shipsToRepair.RemoveAt(i);
                continue;
            }

            shipHealth.Repair(repairRate * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is a player unit and has a Health component
        PlayerUnit playerUnit = other.GetComponent<PlayerUnit>();
        if (playerUnit != null)
        {
            Health shipHealth = other.GetComponent<Health>();
            if (shipHealth != null && !_shipsToRepair.Contains(shipHealth))
            {
                Debug.Log($"{other.name} entered the repair zone.");
                _shipsToRepair.Add(shipHealth);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Health shipHealth = other.GetComponent<Health>();
        if (shipHealth != null && _shipsToRepair.Contains(shipHealth))
        {
            Debug.Log($"{other.name} exited the repair zone.");
            _shipsToRepair.Remove(shipHealth);
        }
    }
}
