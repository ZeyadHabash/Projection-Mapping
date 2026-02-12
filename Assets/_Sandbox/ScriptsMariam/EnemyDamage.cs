using _Sandbox.Scripts.IdentityCore;
using _Sandbox.Scripts.Managers;
using UnityEngine;

namespace _Sandbox.ScriptsMariam
{
    public class EnemyDamage : MonoBehaviour
    {

        [SerializeField] private AudioClip destroySound;
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<CoreHealth>().TakeDamage();
                if (AudioFXManager.Instance != null) AudioFXManager.Instance.PlayFXClip(destroySound);
                Destroy(gameObject);
            }
        }
    }
}
