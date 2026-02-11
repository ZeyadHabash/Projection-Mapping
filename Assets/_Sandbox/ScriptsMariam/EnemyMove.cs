using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float speed = 2f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 target = new Vector3(player.position.x, player.position.y, transform.position.z);
        Vector3 dir = (target - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;
    }

}
