using UnityEngine;
using System.Collections;

public class CylinderSpawner : MonoBehaviour
{
    [Header("Cylinder Setup")]
    public GameObject cylinderPrefab;
    public Transform player;

    [Header("Spawn Points")]
    public Transform[] spawnPoints; 
    public Color warningColor = Color.red;
    public float warningTime = 0.3f; 

    [Header("Timing")]
    public float delayBetweenAttacks = 3f; 
    public float minDelayBetweenSpawnPoints = 0.3f;
    public float maxDelayBetweenSpawnPoints = 0.8f;

    [Header("Cylinder Speed")]
    public float cylinderSpeed = 6f;

    [Header("Difficulty Scaling")]
    public float spawnAcceleration = 0.05f;   
    public float speedAcceleration = 0.5f;    
    public float minAttackDelay = 1f;       
    public float maxCylinderSpeed = 12f;      

    private bool firstSpawnDone = false;

    void Start()
    {
        InvokeRepeating(nameof(SpawnPattern), 2f, delayBetweenAttacks);
    }

    void Update()
    {
        
        if (delayBetweenAttacks > minAttackDelay)
        {
            delayBetweenAttacks -= spawnAcceleration * Time.deltaTime;
            delayBetweenAttacks = Mathf.Max(delayBetweenAttacks, minAttackDelay);
        }

       
        if (cylinderSpeed < maxCylinderSpeed)
        {
            cylinderSpeed += speedAcceleration * Time.deltaTime;
            cylinderSpeed = Mathf.Min(cylinderSpeed, maxCylinderSpeed);
        }
    }

    void SpawnPattern()
    {
        if (!firstSpawnDone)
        {
            SpawnFirstHorizontal();
            firstSpawnDone = true;
        }
        else
        {
            StartCoroutine(SpawnFromPointsStaggered());
        }
    }

    void SpawnFirstHorizontal()
    {
       
        Vector3 spawnPos = new Vector3(0f, spawnPoints[0].position.y, -0.5f);

        GameObject cyl = Instantiate(cylinderPrefab, spawnPos, Quaternion.identity);

        StraightLineMove move = cyl.GetComponent<StraightLineMove>();
        move.Initialize(Vector3.right);
        move.speed = cylinderSpeed;
    }

    IEnumerator SpawnFromPointsStaggered()
    {
        foreach (Transform point in spawnPoints)
        {
            
            Renderer rend = point.GetComponent<Renderer>();
            Color originalColor = Color.white;

            if (rend != null)
            {
                originalColor = rend.material.color;
                rend.material.color = warningColor;
            }

            yield return new WaitForSeconds(warningTime);

            if (rend != null)
            {
                rend.material.color = originalColor;
            }

            
            Vector3 playerPos = player.position;
            GameObject cyl = Instantiate(cylinderPrefab, point.position, Quaternion.identity);

            Vector3 direction = (playerPos - point.position).normalized;

            StraightLineMove move = cyl.GetComponent<StraightLineMove>();
            move.Initialize(direction);
            move.speed = cylinderSpeed;

            
            float delay = Random.Range(minDelayBetweenSpawnPoints, maxDelayBetweenSpawnPoints);
            yield return new WaitForSeconds(delay);
        }
    }
}
