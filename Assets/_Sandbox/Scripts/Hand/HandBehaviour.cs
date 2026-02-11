using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities.Extensions;

namespace _Sandbox.Scripts.Hand
{
    public class HandBehaviour : MonoBehaviour
    {
        [SerializeField] [ColorUsage(true, true)] private Color _handColor = Color.cyan;
        [SerializeField] private MeshRenderer ring;
        
        private LensFlareComponentSRP lensFlare;
        private Light handsLight;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private Material ringMat;
        private Sequence closeSequence;
        private Sequence showSequence;
        
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
            SetupCloseSequence();
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

        private void Update() {
            if (test) CloseEffect();
            else OpenEffect();
        }

        public void SetState(float isClosed) {
            if (isClosed > 0) CloseEffect();
            else OpenEffect();
        }

        private void CloseEffect() {
            // if (ring != null) ring.enabled = true;
            // lensFlare.intensity = initLensIntensity + _lensIntensity;
            // lensFlare.scale = initLensScale + _lensScale;
            // light.intensity = initLightIntensity + _lightIntensity;
            closeSequence.PlayForward();
        }

        private void OpenEffect() {
            // if (ring != null) ring.enabled = false;
            // lensFlare.intensity = initLensIntensity;
            // lensFlare.scale = initLensScale;
            // light.intensity = initLightIntensity;
            closeSequence.PlayBackwards();
        }

        public void SetPosition(float x, float y) {
            transform.position = new Vector3(x, y, 0);
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