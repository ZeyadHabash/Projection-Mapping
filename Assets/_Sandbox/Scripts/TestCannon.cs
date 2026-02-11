using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Sandbox.Scripts
{
    public class TestCannon : MonoBehaviour
    {

        private float randomDelay;
        [SerializeField] private AudioClip audioClip;
        
        private void Awake() {
            randomDelay = Random.Range(0, 0.2f);
        }

        private void Start() {
            StartCoroutine(ShootCannonCoroutine());
        }

        private IEnumerator ShootCannonCoroutine() {
            var rb = GetComponent<Rigidbody>();
            yield return new WaitForSeconds(8 + randomDelay);
            var direction = (new Vector3(0, 24, 0) - transform.position).normalized;
            rb.AddForce(direction * 20, ForceMode.Impulse);
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }
    }
}
