using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("Speed Settings")]
    public float speed = 2f;            
    public float acceleration = 0.5f;   
    public float maxSpeed = 5f;         

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        
        if (speed < maxSpeed)
        {
            speed += acceleration * Time.deltaTime;
            if (speed > maxSpeed)
                speed = maxSpeed;
        }

        
        Vector3 target = new Vector3(player.position.x, player.position.y, transform.position.z);
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }
}
