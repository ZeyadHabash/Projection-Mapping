using _Sandbox.Scripts.Hand;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using Utilities.Extensions;

namespace _Sandbox.Scripts.Bubble
{
    public class BubbleEffect : MonoBehaviour
    {

        [SerializeField] private HandBehaviour _rHand; 
        [SerializeField] private HandBehaviour _lHand; 
        [SerializeField] private float _effectDistance = 1.2f;
        [SerializeField] [ColorUsage(true, true)] private Color _defaultColor = Color.gray;

        private TextMeshPro word;
        private VisualEffect vfx;
        private Sequence showSequence;

        private float showDuration = 0.4f;

        [SerializeField] private bool test = false;
        
        private void Awake() {
            vfx = GetComponent<VisualEffect>();
            word = GetComponentInChildren<TextMeshPro>();
            SetupShowSequence();
        }


        private void Update() {
            SetClosestBubbleColor();
            if (test) Show();
            else Hide();
        }

        private void SetupShowSequence() {
            showSequence = DOTween.Sequence();
            showSequence.Join(transform.DOMoveZ(0, showDuration).From(8));
            showSequence.Join(transform.DOScale(1, showDuration).From(0f));
            showSequence.Join(word.DOFade(1, showDuration).From(0));
            showSequence.SetManual(gameObject);
        }

        public void Show() {
            showSequence.timeScale = 1f;
            showSequence.PlayForward();
        }

        public void Hide() {
            showSequence.timeScale = 0.4f;
            showSequence.PlayBackwards();
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