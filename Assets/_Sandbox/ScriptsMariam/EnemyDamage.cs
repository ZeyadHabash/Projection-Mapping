using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().TakeDamage();
            Destroy(gameObject);
        }
    }
}
