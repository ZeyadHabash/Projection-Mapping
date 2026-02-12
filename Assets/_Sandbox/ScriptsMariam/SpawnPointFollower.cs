using UnityEngine;

public class SpawnPointFollower : MonoBehaviour
{
    private Transform player;

    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (followX) { 
            transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
        }

        if (followY) { 
            transform.position = new Vector3(transform.position.x, player.position.y, transform.position.z);
        }
    }
}
