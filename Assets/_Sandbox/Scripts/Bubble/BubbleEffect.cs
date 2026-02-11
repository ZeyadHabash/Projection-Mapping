using _Sandbox.Scripts.Hand;
using UnityEngine;
using UnityEngine.VFX;

namespace _Sandbox.Scripts.Bubble
{
    public class BubbleEffect : MonoBehaviour
    {

        [SerializeField] private HandBehaviour _rHand; 
        [SerializeField] private HandBehaviour _lHand; 
        [SerializeField] private float _effectDistance = 1.2f;
        [SerializeField] [ColorUsage(true, true)] private Color _defaultColor = Color.gray;

        private VisualEffect vfx;

        private void Awake() {
            vfx = GetComponent<VisualEffect>();
        }

        private void Update() {
            SetClosestBubbleColor();
        }

        private void SetClosestBubbleColor() {
            if (_lHand == null || _rHand == null) return;
            var position = transform.position;
            float lDistance = Vector3.Distance(position, _lHand.transform.position);
            float rDistance = Vector3.Distance(position, _rHand.transform.position);
            if (lDistance < rDistance) UpdateBubbleColor(_lHand, lDistance);
            else UpdateBubbleColor(_rHand, rDistance);
        }

        private void UpdateBubbleColor(HandBehaviour hand, float distance) {

            float t = 1.0f - Mathf.Clamp01(distance / (_effectDistance));
            Color finalColor = Color.Lerp(_defaultColor, hand.HandColor, t);
            vfx.SetVector4("color", finalColor);
        }
    }
}