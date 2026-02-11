using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject capsulePrefab;
    public GameObject cylinderPrefab;

    public float spawnRate = 1.5f;

    //Bounds for spawning 
    public Vector2 outerMin;//Gray area 
    public Vector2 outerMax;

    public Vector2 innerMin; //Yellow area
    public Vector2 innerMax;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnRate);
    }

    void SpawnEnemy()
    {
        Vector3 pos = GetRandomGrayPosition();

        int r = Random.Range(0, 3);
        GameObject enemy;

        if (r == 0)
            enemy = Instantiate(cubePrefab, pos, Quaternion.identity);
        else if (r == 1)
            enemy = Instantiate(capsulePrefab, pos, Quaternion.identity);
        else
            enemy = Instantiate(cylinderPrefab, pos, Quaternion.identity);
    }

    Vector3 GetRandomGrayPosition()
    {
        Vector3 pos;

        do
        {
            float x = Random.Range(outerMin.x, outerMax.x);
            float y = Random.Range(outerMin.y, outerMax.y);

            pos = new Vector3(x, y, -0.5f);
        }
        while (pos.x > innerMin.x && pos.x < innerMax.x &&
               pos.y > innerMin.y && pos.y < innerMax.y);

        return pos;
    }

}
