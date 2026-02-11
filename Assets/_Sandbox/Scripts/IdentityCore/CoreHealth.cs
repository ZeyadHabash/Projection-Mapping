using UnityEngine;

namespace _Sandbox.Scripts.IdentityCore
{
    public class CoreHealth : MonoBehaviour
    {
        private Renderer rend;
        private Color originalColor;

        void Awake() {
            rend = GetComponent<Renderer>();
            originalColor = rend.material.color;
        }

        public void TakeDamage() {
            StopAllCoroutines();
            StartCoroutine(FlashRed());
        }

        System.Collections.IEnumerator FlashRed() {
            rend.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            rend.material.color = originalColor;
        }
    }
}