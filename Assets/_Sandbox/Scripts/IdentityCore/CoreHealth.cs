using System.Collections;
using _Sandbox.Scripts.Managers;
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
            if (WordManager.Instance != null) WordManager.Instance.RemoveRandomCollectedWord();
            StopAllCoroutines();
            StartCoroutine(FlashRed());
        }

        IEnumerator FlashRed() {
            rend.material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            rend.material.color = originalColor;
        }
    }
}