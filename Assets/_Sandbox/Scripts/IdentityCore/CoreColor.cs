using _Sandbox.Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

namespace _Sandbox.Scripts.IdentityCore
{
    public class CoreColor : MonoBehaviour
    {
        [Header("Reveal Colors")]
        [SerializeField] [ColorUsage(true, true)] private Color _redGradient;
        [SerializeField] [ColorUsage(true, true)] private Color _yellowGradient;
        [SerializeField] [ColorUsage(true, true)] private Color _greenGradient;
        [SerializeField] [ColorUsage(true, true)] private Color _blueGradient;
        [SerializeField] [ColorUsage(true, true)] private Color _whiteGradient;
        [SerializeField] [ColorUsage(true, true)] private Color _grayGradient;
        
        [SerializeField] private VisualEffect vfx;

        public void RevealColor() {
            // 1. Get the dominant color type
            
            var dominant = WordManager.Instance.GetDominantCollectedColor();

            // 2. Determine which gradient to use
            Color targetGradient = _whiteGradient; // Default fallback
            
            Debug.Log($"--- collection? {dominant}");

            switch (dominant) {
                case WordManager.WordColor.Gray:
                    targetGradient = _grayGradient;
                    break;
                case WordManager.WordColor.Red:
                    targetGradient = _redGradient;
                    break;
                case WordManager.WordColor.Yellow:
                    targetGradient = _yellowGradient;
                    break;
                case WordManager.WordColor.Green:
                    targetGradient = _greenGradient;
                    break;
                case WordManager.WordColor.Blue:
                    targetGradient = _blueGradient;
                    break;
                case WordManager.WordColor.White:
                    targetGradient = _whiteGradient;
                    break;
            }
            
            Debug.Log($"--- set color {targetGradient}");

            if (vfx != null)  vfx.SetVector4("Color", targetGradient);
            transform.DOScale(Vector3.one*2.5f, 0.5f).SetEase(Ease.OutBack);
        }
    }
}