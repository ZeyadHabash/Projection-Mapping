using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Utilities.Extensions
{
    public static class DOExtensions
    {
        public static Sequence SetManual(this Sequence seq, GameObject gameObject) {
            seq.Pause()
                .SetAutoKill(false)
                .SetLink(gameObject);
            
            return seq;
        }
        
        public static Tween SetManual(this Tween tween, GameObject gameObject) {
            tween.Pause()
                .SetAutoKill(false)
                .SetLink(gameObject);
            
            return tween;
        }
        
        public static Tweener DOOffsetMax(this RectTransform target, Vector2 endValue, float duration)
        {
            return DOTween.To(
                () => target.offsetMax,
                x => target.offsetMax = x,
                endValue,
                duration
            );
        }
        
        public static Tweener DOCaretFade(this TMP_InputField inputField, float endValue, float duration)
        {
            return DOTween.To(() => inputField.caretColor.a, 
                x => {
                    Color color = inputField.caretColor;
                    color.a = x;
                    inputField.caretColor = color;
                },
                endValue, 
                duration);
        }
        
        public static Tweener DOFade(this TrailRenderer trail, float endValue, float duration)
        {
            return DOTween.To(() => trail.startColor.a, 
                x => {
                    Color startColor = trail.startColor;
                    Color endColor = trail.endColor;
                    startColor.a = x;
                    endColor.a = x;
                    trail.startColor = startColor;
                    trail.endColor = endColor;
                }, 
                endValue, 
                duration);
        }
        
        public static Tweener DOFade(this LineRenderer trail, float endValue, float duration)
        {
            return DOTween.To(() => trail.startColor.a, 
                x => {
                    Color startColor = trail.startColor;
                    Color endColor = trail.endColor;
                    startColor.a = x;
                    endColor.a = x;
                    trail.startColor = startColor;
                    trail.endColor = endColor;
                }, 
                endValue, 
                duration);
        }
    }
}