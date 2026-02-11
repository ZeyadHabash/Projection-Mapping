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
        [SerializeField][ColorUsage(true, true)] private Color _defaultColor = Color.gray;

        private HandEffect rHand;
        private HandEffect lHand;
        private TextMeshPro word;
        private VisualEffect vfx;
        private Sequence showSequence;

        private float showDuration = 0.4f;
        private const float HideTimeScale = 0.4f;

        public float HideDuration => showDuration / HideTimeScale;

        private void Awake()
        {
            vfx = GetComponent<VisualEffect>();
            word = GetComponentInChildren<TextMeshPro>();
            SetupShowSequence();
        }


        private void Update()
        {
            SetClosestBubbleColor();
        }

        private void SetupShowSequence()
        {
            showSequence = DOTween.Sequence();
            showSequence.Join(transform.DOMoveZ(0, showDuration).From(8));
            showSequence.Join(transform.DOScale(1, showDuration).From(0f));
            showSequence.Join(word.DOFade(1, showDuration).From(0));
            showSequence.SetManual(gameObject);
        }

        public void Init(HandController right, HandController left)
        {
            rHand = right.GetComponent<HandEffect>();
            lHand = right.GetComponent<HandEffect>();
            Show();
        }

        public void Show()
        {
            showSequence.timeScale = 1f;
            showSequence.PlayForward();
        }

        public void Hide()
        {
            showSequence.timeScale = HideTimeScale;
            showSequence.PlayBackwards();
        }

        private void SetClosestBubbleColor()
        {
            if (lHand == null || rHand == null) return;
            var position = transform.position;
            float lDistance = Vector3.Distance(position, lHand.transform.position);
            float rDistance = Vector3.Distance(position, rHand.transform.position);
            if (lDistance < rDistance) UpdateBubbleColor(lHand, lDistance);
            else UpdateBubbleColor(rHand, rDistance);
        }

        private void UpdateBubbleColor(HandEffect hand, float distance)
        {

            float t = 1.0f - Mathf.Clamp01(distance / (_effectDistance));
            Color finalColor = Color.Lerp(_defaultColor, hand.HandColor, t);
            vfx.SetVector4("color", finalColor);
        }
    }
}