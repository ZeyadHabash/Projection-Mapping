using System;
using _Sandbox.Scripts.Enums;
using _Sandbox.Scripts.Hand;
using _Sandbox.Scripts.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using Utilities.Extensions;

namespace _Sandbox.Scripts.Bubble
{
    public class BubbleEffect : MonoBehaviour
    {
        [SerializeField] private BubbleType _bubbleType;
        [SerializeField] private float _effectDistance = 1.2f;
        [SerializeField] [ColorUsage(true, true)] private Color _defaultColor = Color.gray;
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private bool _enableHoverEffect = true;
        [SerializeField] private bool _showOnSpawn = false;
        [SerializeField] private AudioClip[] collectAudioClips;
        
        private HandEffect rHand;
        private HandEffect lHand;
        private HandEffect closestHand;
        private TextMeshPro word;
        private VisualEffect parentVfx;
        private VisualEffect childVFX;
        private bool isGlowing = false;
        private Sequence showSequence;

        private const float HideTimeScale = 0.4f;
        public float HideDuration => _showDuration / HideTimeScale;

        private bool makeParentActive = false;

        private void Awake() {
            parentVfx = GetComponent<VisualEffect>();
            VisualEffect[] allVfx = GetComponentsInChildren<VisualEffect>(); //TODO: I hate this
            foreach (var vfx in allVfx) {
                if (vfx != parentVfx) {
                    childVFX = vfx;
                    break; 
                }
            }
            word = GetComponentInChildren<TextMeshPro>();
            SetupShowSequence();
        }

        private void Start() {
            if (_showOnSpawn) Show();
        }

        private void Update()
        {
            SetClosestBubbleColor();
        }

        private void SetupShowSequence()
        {
            showSequence = DOTween.Sequence();
            transform.position = new Vector3(transform.position.x, transform.position.y, 8);
            transform.localScale = Vector3.zero;
            var curWordColor = word.color;
            curWordColor.a = 0;
            word.color = curWordColor;
            showSequence.Join(transform.DOMoveZ(0, _showDuration));
            showSequence.Join(transform.DOScale(1, _showDuration));
            showSequence.Join(word.DOFade(1, _showDuration));
            showSequence.SetManual(gameObject);
        }

        public void Init(HandController right, HandController left)
        {
            rHand = right.GetComponent<HandEffect>();
            lHand = left.GetComponent<HandEffect>();
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
        
        public void GlowAndDestory() {
            isGlowing = true;
            float glowIntensity = 8.0f;
            Color hdrColor = closestHand.HandColor * glowIntensity;
            DOTween.To(() => Color.clear,
                x => parentVfx.SetVector4("color", x),
                hdrColor,
                _showDuration);
            DOVirtual.DelayedCall(0.1f, () => showSequence.PlayBackwards());
            Destroy(gameObject, _showDuration+0.3f);
        }
        
        public void HideAndDestory()
        {
            showSequence.PlayBackwards();
            Destroy(gameObject, _showDuration+0.2f);
        }
       
        public void Collect() {
            PlayRandomAudio(collectAudioClips);
        }
        
        private void PlayRandomAudio(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
                return;

            if (AudioFXManager.Instance == null)
                return;

            AudioFXManager.Instance.PlayRandomFXClip(clips);
        }

        public void RemoveChild() {
            if (childVFX == null) return;
            childVFX.enabled = false;
            // Vector4 currentHSV = childVFX.GetVector4("color");
            // Vector4 targetHSV = new Vector4(currentHSV.x, currentHSV.y, 0f, currentHSV.w);
            // DOTween.To(() => currentHSV, x => {
            //     currentHSV = x;
            //     childVFX.SetVector4("color", currentHSV);
            // }, targetHSV, 0.3f);
            makeParentActive = true;
        }

        private void SetClosestBubbleColor()
        {
            if (lHand == null || rHand == null || _enableHoverEffect == false) return;
            var position = transform.position;
            float lDistance = Vector3.Distance(position, lHand.transform.position);
            float rDistance = Vector3.Distance(position, rHand.transform.position);
            float closestDistance = 0;
            if (lDistance < rDistance) {
                closestHand = lHand;
                closestDistance = lDistance;
            } else {
                closestHand = rHand;
                closestDistance = rDistance;
            }

            if (_bubbleType == BubbleType.DoubleTap) {
                UpdateBubbleDoubleColor(closestHand, closestDistance);
            } else if (_bubbleType == BubbleType.BothHands) {
                UpdateBubbleBothColor(lDistance, rDistance);
            } else {
                UpdateBubbleColor(closestHand, closestDistance);
            }
        }

        private void UpdateBubbleBothColor(float lDistance, float rDistance) {
            if (isGlowing) return;
            float lt = 1.0f - Mathf.Clamp01(lDistance / (_effectDistance));
            Color lFinalColor = Color.Lerp(_defaultColor, lHand.HandColor, lt);
            float rt = 1.0f - Mathf.Clamp01(rDistance / (_effectDistance));
            Color rFinalColor = Color.Lerp(_defaultColor, rHand.HandColor, rt);
            childVFX.SetVector4("color", rFinalColor);
            parentVfx.SetVector4("color", lFinalColor);
        }

        private void UpdateBubbleDoubleColor(HandEffect hand, float distance) {
            if (isGlowing) return;
            float t = 1.0f - Mathf.Clamp01(distance / (_effectDistance));
            Color finalColor = Color.Lerp(_defaultColor, hand.HandColor, t);
            if (childVFX.enabled && !makeParentActive) {
                childVFX.SetVector4("color", finalColor);
            } else {
                parentVfx.SetVector4("color", finalColor);
            }
        }

        private void UpdateBubbleColor(HandEffect hand, float distance) {
            if (isGlowing) return;
            float t = 1.0f - Mathf.Clamp01(distance / (_effectDistance));
            Color finalColor = Color.Lerp(_defaultColor, hand.HandColor, t);
            parentVfx.SetVector4("color", finalColor);
        }
        
    }
}