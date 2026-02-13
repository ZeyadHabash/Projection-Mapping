using System.Collections;
using _Sandbox.Scripts.Managers;
using UnityEngine;

namespace _Sandbox.Scripts.IdentityCore
{
    public class CoreHealth : MonoBehaviour
    {
        private Renderer rend;
        private Color originalColor;
        [SerializeField] [ColorUsage(true, true)] private Color damageColor = Color.red;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        [SerializeField] private AudioClip damageClip;

        void Awake() {
            rend = GetComponent<Renderer>();
            originalColor = rend.material.color;
        }

        public void TakeDamage() {
            if (WordManager.Instance != null) WordManager.Instance.RemoveRandomCollectedWord();
            StopAllCoroutines();
            StartCoroutine(FlashRed());
            if (AudioFXManager.Instance != null && damageClip != null) AudioFXManager.Instance.PlayFXClip(damageClip);
        }

        IEnumerator FlashRed() {
            rend.material.SetColor(EmissionColor, damageColor);
            rend.material.EnableKeyword("_EMISSION"); 
            yield return new WaitForSeconds(0.2f);
            rend.material.SetColor(EmissionColor, originalColor);
        }
    }
}