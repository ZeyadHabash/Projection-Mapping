using _Sandbox.Scripts.Managers;
using UnityEngine;
using UnityEngine.VFX;

namespace _Sandbox.Scripts.IdentityCore
{
    public class CoreColor : MonoBehaviour
    {
        [Header("Reveal Colors")]
        [SerializeField] [GradientUsage(true)] private Gradient _redGradient;
        [SerializeField] [GradientUsage(true)] private Gradient _yellowGradient;
        [SerializeField] [GradientUsage(true)] private Gradient _greenGradient;
        [SerializeField] [GradientUsage(true)] private Gradient _blueGradient;
        [SerializeField] [GradientUsage(true)] private Gradient _whiteGradient;

        private VisualEffect vfx;

        private void Awake() {
            vfx = GetComponentInChildren<VisualEffect>();
        }

        public void RevealColor() {
            // 1. Get the dominant color type
            var dominant = WordManager.Instance.GetDominantCollectedColor();

            // 2. Determine which gradient to use
            Gradient targetGradient = _whiteGradient; // Default fallback

            switch (dominant) {
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

            Color sampledColor = targetGradient.Evaluate(1.0f);

            if (vfx != null)  vfx.SetVector4("MainColor", sampledColor);
        }
    }
}