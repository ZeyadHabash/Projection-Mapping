using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities.Extensions;
using Sequence = DG.Tweening.Sequence;

namespace _Sandbox.Scripts.Managers
{
    public class CollectionSceneManager : MonoBehaviour
    {
        [SerializeField] private MeshRenderer grid;
        [SerializeField] private TextMeshPro sceneText;
        [SerializeField] private float transitionDuraiton = 0.8f;
        [SerializeField] private float maxVolume = 0.8f;
        [SerializeField] [ColorUsage(true, true)] private Color _gridColor = Color.gray;
        
        private Sequence appearSequence;
        private Material gridMat;
        private static readonly int Fade = Shader.PropertyToID("_fade");

        public static Action OnEndSceneAction;

        private void Awake() {
            gridMat = grid.material;
            SetupAppearSequence();
        }

        private void Start() {
            Appear();
        }

        private void SetupAppearSequence() {
            appearSequence = DOTween.Sequence();
            appearSequence.Join(DOTween.To(
                () => 0f,                               
                x => AudioMusicManager.Instance.SetVolume(x), 
                maxVolume,                                  
                transitionDuraiton*2.5f                     
            ).SetEase(Ease.InQuad));
            appearSequence.Join(gridMat.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.Join(gridMat.DOColor(_gridColor, transitionDuraiton).From(0f));
            appearSequence.Join(gridMat.DOFloat(6f, Fade, transitionDuraiton).From(24f));
            appearSequence.Join(sceneText.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.SetManual(gameObject);
            
            // TODO: next scene --- call scene manager change grid material color to that nice shade of blue <3
        }

        private void Appear() {
            // do appear logic...
            appearSequence.PlayForward();
        }

        private void Hide() {
            appearSequence.timeScale = 3f;
            appearSequence.PlayBackwards();
        }
        
        private void HandleSceneEnd() {
            Hide();
            StartCoroutine(GoToNextScene());
        }

        IEnumerator GoToNextScene() {
            yield return new WaitForSeconds(transitionDuraiton/3f);
            Debug.Log("next scene");
        }

        private void NextScene() {
            
        }

        private void OnEnable() {
            CollectionSceneManager.OnEndSceneAction += HandleSceneEnd;
        }
       
        private void OnDisable() {
            CollectionSceneManager.OnEndSceneAction -= HandleSceneEnd;
        }
    }
}
