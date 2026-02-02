using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ShipMotor))]
public class CargoShipAI : MonoBehaviour
{
    private ShipMotor _shipMotor;
    private ShippingLane _shippingLane;
    private int _currentWaypointIndex;
    private bool _isInitialized = false;

    private const float ArrivalThreshold = 5.0f;

    void Awake()
    {
        _shipMotor = GetComponent<ShipMotor>();
    }

    public void Initialize(ShippingLane lane)
    {
        if (lane == null || lane.Waypoints.Count == 0)
        {
            Debug.LogError("Failed to initialize CargoShipAI: Shipping lane is invalid.", gameObject);
            Destroy(gameObject);
            return;
        }

        _shippingLane = lane;
        _currentWaypointIndex = 0;
        
        // The spawner now places the ship at the first waypoint.
        // We just need to start moving towards it.
        Transform startingWaypoint = _shippingLane.Waypoints[_currentWaypointIndex];
        _shipMotor.MoveTo(startingWaypoint.position);
        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized) return;

        // Check if the cargo ship has reached the current waypoint
        if (_shipMotor.HasReachedDestination())
        {
            _currentWaypointIndex++;
            
            // Check if we have reached the end of the lane
            if (_currentWaypointIndex >= _shippingLane.Waypoints.Count)
            {
                ArriveAtPort();
            }
            else
            {
                // Set the next waypoint as the destination
                Transform nextWaypoint = _shippingLane.Waypoints[_currentWaypointIndex];
                _shipMotor.MoveTo(nextWaypoint.position);
            }
        }
    }

    private void ArriveAtPort()
    {
        // Notify GameManager to grant credits
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCredits(100); // Example value
        }

        // Self-destruct
        Destroy(gameObject);
    }
}

