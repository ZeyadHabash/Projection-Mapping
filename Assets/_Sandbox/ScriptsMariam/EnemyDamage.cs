using _Sandbox.Scripts.IdentityCore;
using UnityEngine;

namespace _Sandbox.ScriptsMariam
{
    public class EnemyDamage : MonoBehaviour
    {

        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<CoreHealth>().TakeDamage();
                Destroy(gameObject);
            }
        }
    }
}
