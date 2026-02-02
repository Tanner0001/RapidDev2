using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [Header("Cargo Ships")]
    [SerializeField] private List<GameObject> cargoShipPrefabs;
    [SerializeField] private List<ShippingLane> shippingLanes;
    [SerializeField] private float cargoSpawnInterval = 15f;

    [Header("Enemies")]
    [SerializeField] private List<GameObject> enemyShipPrefabs;
    [SerializeField] private List<Transform> enemySpawnPoints;
    [SerializeField] private float enemySpawnInterval = 20f;


    void Start()
    {
        if (cargoShipPrefabs.Count == 0 || shippingLanes.Count == 0)
        {
            // Debug.LogError removed
        }
        else
        {
            StartCoroutine(SpawnCargoShipsCoroutine());
        }
        
        if (enemyShipPrefabs.Count == 0 || enemySpawnPoints.Count == 0)
        {
            // Debug.LogError removed
        }
        else
        {
            StartCoroutine(SpawnEnemiesCoroutine());
        }
    }

    private IEnumerator SpawnCargoShipsCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(cargoSpawnInterval);

            // Pick a random lane and prefab
            ShippingLane lane = shippingLanes[Random.Range(0, shippingLanes.Count)];
            if (lane == null || lane.Waypoints.Count == 0)
            {
                Debug.LogWarning("Skipping cargo ship spawn because a shipping lane is invalid.");
                continue;
            }

            GameObject prefabToSpawn = cargoShipPrefabs[Random.Range(0, cargoShipPrefabs.Count)];
            Transform spawnPoint = lane.Waypoints[0];

            // Spawn at the first waypoint of the lane
            GameObject spawnedShip = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation); 
            CargoShipAI cargoAI = spawnedShip.GetComponent<CargoShipAI>();
            if (cargoAI != null)
            {
                cargoAI.Initialize(lane);
            }
            else
            {
                Debug.LogError($"Prefab {prefabToSpawn.name} is missing CargoShipAI component.", spawnedShip);
                Destroy(spawnedShip);
            }
        }
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {

        while (true)
        {
            yield return new WaitForSeconds(enemySpawnInterval);
            
            // Pick a random spawn point and prefab
            Transform spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
            if (spawnPoint == null)
            {
                continue;
            }

            GameObject prefabToSpawn = enemyShipPrefabs[Random.Range(0, enemyShipPrefabs.Count)];
            if (prefabToSpawn == null)
            {
                continue;
            }
            if (NavMesh.SamplePosition(spawnPoint.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {

                Instantiate(prefabToSpawn, hit.position, spawnPoint.rotation);

            }

        }
    }
}