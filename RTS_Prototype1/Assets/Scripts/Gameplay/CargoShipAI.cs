using UnityEngine;

[RequireComponent(typeof(ShipMotor))]
public class CargoShipAI : MonoBehaviour
{
    private ShipMotor _shipMotor;
    private Transform _destination;
    private bool _isInitialized = false;

    private const float ArrivalThreshold = 5.0f;

    void Awake()
    {
        _shipMotor = GetComponent<ShipMotor>();
    }

    public void Initialize(Transform destination)
    {
        _destination = destination;
        _shipMotor.MoveTo(_destination.position);
        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized || _destination == null) return;

        // Check if the cargo ship has reached the home port
        if (Vector3.Distance(transform.position, _destination.position) < ArrivalThreshold)
        {
            ArriveAtPort();
        }
    }

    private void ArriveAtPort()
    {
        Debug.Log($"{name} has arrived at the port!");

        // Notify GameManager to grant credits
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCredits(100); // Example value
        }

        // Self-destruct
        Destroy(gameObject);
    }
}
