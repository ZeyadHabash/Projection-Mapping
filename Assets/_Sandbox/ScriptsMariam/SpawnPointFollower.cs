using UnityEngine;

public class SpawnPointFollower : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

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
