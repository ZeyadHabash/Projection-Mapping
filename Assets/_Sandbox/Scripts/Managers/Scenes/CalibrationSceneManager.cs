using System;
using System.Collections;
using _Sandbox.Scripts.Enums;
using _Sandbox.Scripts.Hand;
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
        [SerializeField] private AudioClip sceneMusic;


        private float musicVolume = 0.025f;
        
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
            OSCManager.Instance.LeftHand.GetComponent<HandEffect>().ResetColor();
            OSCManager.Instance.RightHand.GetComponent<HandEffect>().ResetColor();
            if (OSCManager.Instance != null) OSCManager.Instance.ResetCore();
            if (AudioMusicManager.Instance != null) {
                AudioMusicManager.Instance.SwitchMusic(sceneMusic);
                AudioMusicManager.Instance.FadeMusic(transitionDuraiton*2, musicVolume);
            }
            appearSequence.PlayForward();
        }

        private void Hide() {
            appearSequence.timeScale = 3f;
            if (AudioMusicManager.Instance != null) AudioMusicManager.Instance.FadeMusic(transitionDuraiton, 0f);
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
