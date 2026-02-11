using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities.Extensions;

namespace _Sandbox.Scripts.Managers
{
    public class CalibrationSceneManager : MonoBehaviour
    {
        [SerializeField] private MeshRenderer grid;
        [SerializeField] private TextMeshPro welcomeText;
        [SerializeField] private float transitionDuraiton = 0.8f;
        
        private Sequence appearSequence;
        private Material gridMat;
        private static readonly int Fade = Shader.PropertyToID("_fade");

        [SerializeField] private bool test = false;

        private void Awake() {
            gridMat = grid.material;
            SetupAppearSequence();
        }

        private void Update() {
            if (test) Appear();
            else Hide();
        }

        private void SetupAppearSequence() {
            appearSequence = DOTween.Sequence();
            //TODO: fade in music
            appearSequence.Join(gridMat.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.Join(gridMat.DOFloat(6f, Fade, transitionDuraiton).From(24f));
            appearSequence.Join(welcomeText.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.SetManual(gameObject);
        }

        private void Appear() {
            // do appear logic...
            appearSequence.PlayForward();
        }

        private void Hide() {
            appearSequence.PlayBackwards();
        }
    }
}
