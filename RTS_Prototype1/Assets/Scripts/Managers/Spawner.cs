using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [Header("Cargo Ships")]
    [SerializeField] private List<GameObject> cargoShipPrefabs;
    [SerializeField] private List<Transform> cargoSpawnPoints;
    [SerializeField] private Transform homePort;
    [SerializeField] private float cargoSpawnInterval = 15f;

    [Header("Enemies")]
    [SerializeField] private List<GameObject> enemyShipPrefabs;
    [SerializeField] private List<Transform> enemySpawnPoints;
    [SerializeField] private float enemySpawnInterval = 20f;


    void Start()
    {
        if (cargoShipPrefabs.Count == 0 || cargoSpawnPoints.Count == 0 || homePort == null)
        {
            Debug.LogError("Spawner: Cargo ship spawning not configured properly.", this);
        }
        else
        {
            StartCoroutine(SpawnCargoShipsCoroutine());
        }
        
        if (enemyShipPrefabs.Count == 0 || enemySpawnPoints.Count == 0)
        {
            Debug.LogError("Spawner: Enemy spawning not configured properly.", this);
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

            // Pick a random spawn point and prefab
            Transform spawnPoint = cargoSpawnPoints[Random.Range(0, cargoSpawnPoints.Count)];
            GameObject prefabToSpawn = cargoShipPrefabs[Random.Range(0, cargoShipPrefabs.Count)];

            // Spawn and initialize
            GameObject spawnedShip = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            CargoShipAI cargoAI = spawnedShip.GetComponent<CargoShipAI>();
            if (cargoAI != null)
            {
                cargoAI.Initialize(homePort);
            }
            else
            {
                Debug.LogWarning($"Spawner: Spawned cargo ship {prefabToSpawn.name} is missing CargoShipAI component. Destroying.", this);
                Destroy(spawnedShip);
            }
        }
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        Debug.Log("Enemy spawning coroutine started.");
        while (true)
        {
            yield return new WaitForSeconds(enemySpawnInterval);
            
            // Pick a random spawn point and prefab
            Transform spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
            if (spawnPoint == null)
            {
                Debug.LogError("Spawner: An enemy spawn point in the list is null. Skipping this spawn.", this);
                continue;
            }

            GameObject prefabToSpawn = enemyShipPrefabs[Random.Range(0, enemyShipPrefabs.Count)];
            if (prefabToSpawn == null)
            {
                Debug.LogError("Spawner: An enemy ship prefab in the list is null. Skipping this spawn.", this);
                continue;
            }

            // Find a valid position on the NavMesh near the spawn point
            if (NavMesh.SamplePosition(spawnPoint.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                Debug.Log($"Spawner: Attempting to spawn enemy '{prefabToSpawn.name}' at {hit.position}.");
                Instantiate(prefabToSpawn, hit.position, spawnPoint.rotation);
                Debug.Log($"Spawner: Instantiation of '{prefabToSpawn.name}' complete.");
            }
            else
            {
                Debug.LogWarning($"Spawner: Could not find a valid NavMesh position near the enemy spawn point '{spawnPoint.name}'. Please check its position. Skipping this spawn.", this);
            }
        }
    }
}