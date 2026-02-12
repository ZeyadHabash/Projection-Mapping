using System;
using System.Collections;
using _Sandbox.Scripts.Enums;
using _Sandbox.ScriptsMariam;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities.Extensions;
using Sequence = DG.Tweening.Sequence;

namespace _Sandbox.Scripts.Managers
{
    public class CalibrationSceneManager : MonoBehaviour
    {
        [SerializeField] private MeshRenderer grid;
        [SerializeField] private TextMeshPro sceneText;
        [SerializeField] private float transitionDuraiton = 0.8f;
        [SerializeField] private MeshRenderer handOpenedRenderer;
        [SerializeField] [ColorUsage(true, true)] private Color _gridColor = Color.gray;
        
        private Sequence appearSequence;
        private Material gridMat;
        private Material handClosedMat;
        private Material handOpenedMat;
        private static readonly int Fade = Shader.PropertyToID("_fade");

        public static Action OnEndSceneAction;

        private void Awake() {
            gridMat = grid.material;
            handOpenedMat = handOpenedRenderer.sharedMaterial;
            SetupAppearSequence();
        }

        private void Start() {
            Appear();
            Invoke("HandleSceneEnd", 2f);
        }
        
        private void SetupAppearSequence() {
            appearSequence = DOTween.Sequence();
            appearSequence.Join(gridMat.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.Join(gridMat.DOColor(_gridColor, transitionDuraiton).From(0f));
            appearSequence.Join(handOpenedMat.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.Join(gridMat.DOFloat(6f, Fade, transitionDuraiton).From(24f));
            appearSequence.Join(sceneText.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.SetManual(gameObject);
            
            // TODO: next scene --- call scene manager change grid material color to that nice shade of blue <3
        }

        private void Appear() {
            AudioMusicManager.Instance.FadeMusic(transitionDuraiton*2, 0.7f);
            appearSequence.PlayForward();
        }

        private void Hide() {
            appearSequence.timeScale = 3f;
            AudioMusicManager.Instance.FadeMusic(transitionDuraiton, 0f);
            appearSequence.PlayBackwards();
        }
        
        private void HandleSceneEnd() {
            Hide();
            StartCoroutine(GoToNextScene());
        }

        IEnumerator GoToNextScene() {
            yield return new WaitForSeconds(transitionDuraiton/2f);
            GameSceneManager.Instance.LoadScene(SceneEnum.CollectionScene);
        }

        private void OnEnable() {
            CalibrationSceneManager.OnEndSceneAction += HandleSceneEnd;
        }
       
        private void OnDisable() {
            CalibrationSceneManager.OnEndSceneAction -= HandleSceneEnd;
        }
    }
}
