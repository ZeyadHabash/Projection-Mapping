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

        [SerializeField] private float _effectDistance = 1.2f;
        [SerializeField] [ColorUsage(true, true)] private Color _defaultColor = Color.gray;
        [SerializeField] private float _showDuration = 0.5f; 

        private HandBehaviour rHand; 
        private HandBehaviour lHand; 
        private TextMeshPro word;
        private VisualEffect parentVFX;
        private VisualEffect childVFX;
        private Sequence showSequence;

        private void Awake() {
            parentVFX = GetComponent<VisualEffect>();
            childVFX = GetComponentInChildren<VisualEffect>();
            word = GetComponentInChildren<TextMeshPro>();
            SetupShowSequence();
        }


        private void Update() {
            SetClosestBubbleColor();
        }

        private void SetupShowSequence() {
            showSequence = DOTween.Sequence();
            showSequence.Join(transform.DOMoveZ(0, _showDuration).From(8));
            showSequence.Join(transform.DOScale(1, _showDuration).From(0f));
            showSequence.Join(word.DOFade(1, _showDuration).From(0));
            showSequence.SetManual(gameObject);
        }

        public void Init(HandController right, HandController left) {
            rHand = right.GetComponent<HandBehaviour>();
            lHand = right.GetComponent<HandBehaviour>();
            Show();
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
            if (lHand == null || rHand == null) return;
            var position = transform.position;
            float lDistance = Vector3.Distance(position, lHand.transform.position);
            float rDistance = Vector3.Distance(position, rHand.transform.position);
            if (lDistance < rDistance) UpdateBubbleColor(lHand, lDistance);
            else UpdateBubbleColor(rHand, rDistance);
        }

        private void UpdateBubbleColor(HandBehaviour hand, float distance) {
            float t = 1.0f - Mathf.Clamp01(distance / (_effectDistance));
            Color finalColor = Color.Lerp(_defaultColor, hand.HandColor, t);
            parentVFX.SetVector4("color", finalColor);
            if (childVFX != null) childVFX.SetVector4("color", finalColor); //TODO: I hate this... when double.. this should be .. I hate this..
        }
    }
}