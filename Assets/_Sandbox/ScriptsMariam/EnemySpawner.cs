using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] prefabsToSpawn; 

    [Header("Spawn Timing")]
    [SerializeField] private float startSpawnDelay = 3f;
    [SerializeField] private float minSpawnDelay = 0.8f;
    [SerializeField] private float accelerationRate = 0.05f;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 outerMin;
    [SerializeField] private Vector2 outerMax;
    [SerializeField] private Vector2 innerMin;
    [SerializeField] private Vector2 innerMax;

    [Header("Grid Settings")]
    [SerializeField] private int gridColumns = 5;
    [SerializeField] private int gridRows = 5;

    private float currentSpawnDelay;
    private List<Vector3> spawnPositions = new List<Vector3>();
    private int spawnIndex = 0;
    private int prefabIndex = 0;

    void Start()
    {
        currentSpawnDelay = startSpawnDelay;
        GenerateGridPositions();
        StartCoroutine(SpawnLoop());
    }

    
    void GenerateGridPositions()
    {
        spawnPositions.Clear();

        float xStep = (outerMax.x - outerMin.x) / (gridColumns - 1);
        float yStep = (outerMax.y - outerMin.y) / (gridRows - 1);

        for (int i = 0; i < gridColumns; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                float x = outerMin.x + i * xStep;
                float y = outerMin.y + j * yStep;

                
                if (x > innerMin.x && x < innerMax.x && y > innerMin.y && y < innerMax.y)
                    continue;

                spawnPositions.Add(new Vector3(x, y, -0.5f));
            }
        }
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(currentSpawnDelay);

            if (currentSpawnDelay > minSpawnDelay)
            {
                currentSpawnDelay -= accelerationRate;
                if (currentSpawnDelay < minSpawnDelay)
                    currentSpawnDelay = minSpawnDelay;
            }
        }
    }

    void SpawnEnemy()
    {
        if (spawnPositions.Count == 0) return;

        
        Vector3 pos = spawnPositions[spawnIndex];
        spawnIndex = (spawnIndex + 1) % spawnPositions.Count;

       
        GameObject prefab = prefabsToSpawn[prefabIndex];
        prefabIndex = (prefabIndex + 1) % prefabsToSpawn.Length;

        Instantiate(prefab, pos, Quaternion.identity);
    }
}
