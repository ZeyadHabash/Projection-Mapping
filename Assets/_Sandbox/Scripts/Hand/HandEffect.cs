using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities.Extensions;

namespace _Sandbox.Scripts.Hand
{
    public class HandEffect : MonoBehaviour
    {
        [SerializeField] [ColorUsage(true, true)] private Color _handColor = Color.cyan;
        [SerializeField] private MeshRenderer ring;
        [SerializeField] private MeshRenderer sphere;
        
        private LensFlareComponentSRP lensFlare;
        private Light handsLight;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private Material ringMat;
        private Material sphereMat;
        private Sequence closeSequence;
        private Sequence hideSequence;
        
        [Header("Close Effect")]
        [SerializeField] private float _lensIntensity = 0.2f; 
        [SerializeField] private float _lensScale = 0.2f; 
        [SerializeField] private float _lightIntensity = 0.3f; 
        [SerializeField] private float _duration = 0.2f; 
        
        [SerializeField] private bool test = false;

        public Color HandColor => _handColor;
        
        private void Awake() {
            lensFlare = GetComponentInChildren<LensFlareComponentSRP>();
            handsLight = GetComponentInChildren<Light>();
            ringMat = ring.material;
            sphereMat = sphere.material;
            SetupCloseSequence();
            SetupHideSequence();
        }
        
        private void SetupCloseSequence() {
            closeSequence = DOTween.Sequence();

            closeSequence.Join(DOTween.To(() => lensFlare.intensity, 
                x => lensFlare.intensity = x, 
                _lensIntensity, 
                _duration).SetRelative(true));

            closeSequence.Join(DOTween.To(() => lensFlare.scale, 
                x => lensFlare.scale = x, 
                _lensScale, 
                _duration).SetRelative(true));

            closeSequence.Join(handsLight.DOIntensity(_lightIntensity, _duration).SetRelative(true));
            
            closeSequence.Join(ringMat.DOFade(1f, _duration));

            closeSequence.SetManual(gameObject);
        }
        
                
        private void SetupHideSequence() {
            hideSequence = DOTween.Sequence();

            hideSequence.Join(DOTween.To(() => lensFlare.intensity, 
                x => lensFlare.intensity = x, 
                0, 
                _duration));

            hideSequence.Join(DOTween.To(() => lensFlare.scale, 
                x => lensFlare.scale = x, 
                0, 
                _duration));

            hideSequence.Join(handsLight.DOIntensity(0, _duration));

            hideSequence.Join(sphereMat.DOFade(0, _duration));
            
            hideSequence.SetManual(gameObject);
        }

        public void SetState(float isClosed) {
            if (isClosed > 0) CloseEffect();
            else OpenEffect();
        }

        private void CloseEffect() {
            closeSequence.PlayForward();
        }

        private void OpenEffect() {
            closeSequence.PlayBackwards();
        }
        
        private void OnValidate() {
            var tmpRenderer = GetComponentInChildren<Renderer>();
            var tmpLight = GetComponentInChildren<Light>();
            if (tmpRenderer != null)
            {
                tmpRenderer.sharedMaterial.SetColor(BaseColor, _handColor);
                tmpRenderer.sharedMaterial.SetColor(EmissionColor, _handColor);
            }

            if (tmpLight != null)
            {
                tmpLight.color = _handColor;
            }
        }
    }
}