using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(ShipMotor))]
public class AutoHailer : MonoBehaviour
{
    private ShipMotor _shipMotor;
    private SphereCollider _hailTrigger;

    void Awake()
    {
        _shipMotor = GetComponent<ShipMotor>();
        _hailTrigger = GetComponent<SphereCollider>();

        // Configure the trigger
        _hailTrigger.isTrigger = true;
        // The hail distance is 15 in ShipMotor, let's match that.
        // We can make this a variable later if needed.
        _hailTrigger.radius = 15.0f; 
    }

    void OnTriggerEnter(Collider other)
    {
        // We only want to auto-hail if the ship is idle.
        if (_shipMotor.CurrentState != UnitState.Idle)
        {
            return;
        }

        // We only care about objects with a ShipIdentity
        ShipIdentity targetIdentity = other.GetComponent<ShipIdentity>();
        if (targetIdentity != null)
        {
            // Do not auto-hail ships that are already known to be friendly
            if (targetIdentity.IsRevealed && targetIdentity.RealIdentity == ThreatLevel.Green)
            {
                return;
            }

            Debug.Log($"AutoHailer on {name} detected {other.name} while idle. Ordering scan.");
            // Tell our ship to scan this target.
            _shipMotor.Scan(targetIdentity.transform);
        }
    }
}
