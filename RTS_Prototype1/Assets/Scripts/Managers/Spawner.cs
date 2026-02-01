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