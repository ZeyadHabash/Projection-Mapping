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
    public class ProtectionSceneManager : MonoBehaviour
    {
        [SerializeField] private MeshRenderer grid;
        [SerializeField] private TextMeshPro sceneText;
        [SerializeField] private float transitionDuraiton = 0.8f;
        [SerializeField] [ColorUsage(true, true)] private Color _gridColor = Color.gray;
        
        private Sequence appearSequence;
        private Material gridMat;
        private static readonly int Fade = Shader.PropertyToID("_fade");

        public static Action OnEndSceneAction;

        private void Awake() {
            gridMat = grid.material;
            SetupAppearSequence();
            OSCManager.Instance.LeftHand.GetComponent<HandEffect>().SetDefendColor();
            OSCManager.Instance.RightHand.GetComponent<HandEffect>().SetDefendColor();
        }

        private void Start() {
            Appear();
            // StartCoroutine(SceneTimerCoroutine());
            // Invoke("HandleSceneEnd", 6f);
        }
        
        private void SetupAppearSequence() {
            appearSequence = DOTween.Sequence();
            appearSequence.Join(gridMat.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.Join(gridMat.DOColor(_gridColor, transitionDuraiton).From(0f));
            appearSequence.Join(gridMat.DOFloat(6f, Fade, transitionDuraiton).From(24f));
            appearSequence.Join(sceneText.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.SetManual(gameObject);
            
            // TODO: next scene --- call scene manager change grid material color to that nice shade of blue <3
        }

        private void Appear() {
            if (AudioFXManager.Instance != null) AudioMusicManager.Instance.FadeMusic(transitionDuraiton*2, 0.7f);
            appearSequence.PlayForward();
        }

        private void Hide() {
            appearSequence.timeScale = 3f;
            if (AudioFXManager.Instance != null) AudioMusicManager.Instance.FadeMusic(transitionDuraiton, 0f);
            appearSequence.PlayBackwards();
        }
        
        private void HandleSceneEnd() {
            Hide();
            StartCoroutine(GoToNextScene());
        }
        
        private IEnumerator SceneTimerCoroutine() {
            yield return new WaitForSeconds(60f);
            HandleSceneEnd();
        }

        IEnumerator GoToNextScene() {
            yield return new WaitForSeconds(transitionDuraiton/2f);
            GameSceneManager.Instance.LoadScene(SceneEnum.RevealScene);
        }
        
        private void OnEnable() {
            ProtectionSceneManager.OnEndSceneAction += HandleSceneEnd;
        }
       
        private void OnDisable() {
            ProtectionSceneManager.OnEndSceneAction -= HandleSceneEnd;
        }

    }
}
