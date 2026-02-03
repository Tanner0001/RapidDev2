using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DestinationZone : MonoBehaviour
{
    private void Awake()
    {
        // Ensure the collider is a trigger
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has a CargoShipAI component
        CargoShipAI cargoShip = other.GetComponent<CargoShipAI>();
        if (cargoShip != null)
        {
            Debug.Log($"{other.name} entered a destination zone. Telling it to arrive at port.");
            // Call the ArriveAtPort method on the cargo ship
            cargoShip.ArriveAtPort();
        }
    }
}
