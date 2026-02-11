using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

namespace _Sandbox.Scripts.Bubble
{
    public class BubbleEffect : MonoBehaviour
    {
        // private Tween colorTween;

        [SerializeField] private float _effectDistance = 1.2f;
        [SerializeField] [ColorUsage(true, true)] private Color _defaultColor = Color.gray;
        [SerializeField] [ColorUsage(true, true)] private Color _effectColor = Color.cyan;
        [SerializeField] private Transform _target; 

        private VisualEffect vfx;

        private void Awake() {
            vfx = GetComponent<VisualEffect>();
        }

        private void Update() {
            UpdateBubbleColor();
        }

        private void UpdateBubbleColor() {
            if (_target == null) return;

            float distance = Vector3.Distance(transform.position, _target.position);
            float t = 1.0f - Mathf.Clamp01(distance / _effectDistance);
            Color finalColor = Color.Lerp(_defaultColor, _effectColor, t);

            vfx.SetVector4("color", finalColor);
        }
    }
}