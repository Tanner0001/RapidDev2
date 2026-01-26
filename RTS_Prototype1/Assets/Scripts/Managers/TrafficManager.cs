
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrafficManager : MonoBehaviour
{
    [SerializeField] private List<ShippingLaneRenderer> shippingLaneRenderers;
    [SerializeField] private List<GameObject> shipPrefabs;
    [SerializeField] private GameObject badGuyPrefab; // The specific prefab for the "bad guy"
    [SerializeField, Range(0f, 1f)] private float badGuySpawnChance = 0.2f; // 20% chance to spawn a bad guy

    [SerializeField] private float minSpawnTime = 5.0f;
    [SerializeField] private float maxSpawnTime = 10.0f;

    void Start()
    {
        if (shippingLaneRenderers == null || shippingLaneRenderers.Count == 0 || (shipPrefabs.Count == 0 && badGuyPrefab == null))
        {
            Debug.LogError("TrafficManager: Shipping Lane Renderers or Ship Prefabs are not configured. Disabling spawning.", this);
            return;
        }

        StartCoroutine(SpawnTrafficCoroutine());
    }

    private IEnumerator SpawnTrafficCoroutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            ShippingLaneRenderer randomLaneRenderer = shippingLaneRenderers[Random.Range(0, shippingLaneRenderers.Count)];
            GameObject prefabToSpawn;
            ThreatLevel assignedIdentity;

            // Decide whether to spawn a bad guy or a civilian ship
            if (Random.value < badGuySpawnChance && badGuyPrefab != null)
            {
                prefabToSpawn = badGuyPrefab;
                assignedIdentity = ThreatLevel.Red;
            }
            else
            {
                prefabToSpawn = shipPrefabs[Random.Range(0, shipPrefabs.Count)];
                assignedIdentity = ThreatLevel.Green;
            }

            if (randomLaneRenderer.lanePoints == null || randomLaneRenderer.lanePoints.Count < 2 || randomLaneRenderer.lanePoints[0] == null)
            {
                Debug.LogWarning("TrafficManager: A selected ShippingLaneRenderer has insufficient or null lane points. Skipping spawn.", this);
                continue;
            }

            Vector3 spawnPosition = randomLaneRenderer.lanePoints[0].position;
            NavMeshHit navHit;
            if (!NavMesh.SamplePosition(spawnPosition, out navHit, 10.0f, NavMesh.AllAreas))
            {
                Debug.LogWarning($"TrafficManager: Could not find a valid NavMesh position near the first point of '{randomLaneRenderer.name}'. Please check its position. Skipping spawn.", this);
                continue;
            }

            GameObject spawnedShip = Instantiate(prefabToSpawn, navHit.position, randomLaneRenderer.lanePoints[0].rotation);
            ShipMotor shipMotor = spawnedShip.GetComponent<ShipMotor>();
            
            if (shipMotor != null)
            {
                ShipIdentity shipIdentity = spawnedShip.GetComponent<ShipIdentity>();
                if (shipIdentity != null)
                {
                    shipIdentity.InitializeIdentity(assignedIdentity);
                }
                else
                {
                    Debug.LogWarning($"TrafficManager: Spawned prefab {prefabToSpawn.name} does not have a ShipIdentity component. Identity will not be set.", this);
                }

                shipMotor.FollowLane(randomLaneRenderer.lanePoints);
            }
            else
            {
                Debug.LogWarning($"TrafficManager: Spawned prefab {prefabToSpawn.name} does not have a ShipMotor component! Destroying spawned object.", this);
                Destroy(spawnedShip);
            }
        }
    }
}

