using _Sandbox.Scripts.IdentityCore;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities.Extensions;
using Sequence = DG.Tweening.Sequence;

namespace _Sandbox.Scripts.Managers
{
    public class RevealSceneManager : MonoBehaviour
    {
        [SerializeField] private MeshRenderer grid;
        [SerializeField] private TextMeshPro sceneText;
        [SerializeField] private float transitionDuraiton = 0.8f;
        [SerializeField] [ColorUsage(true, true)] private Color _gridColor = Color.gray;
        [SerializeField] private AudioClip sceneMusic;

        private Transform player; 
        private Sequence appearSequence;
        private Material gridMat;
        private static readonly int Fade = Shader.PropertyToID("_fade");

        private void Awake() {
            gridMat = grid.material;
            SetupAppearSequence();
        }

        private void Start() {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            Appear();
        }
        
        private void SetupAppearSequence() {
            appearSequence = DOTween.Sequence();
            appearSequence.Join(gridMat.DOFade(0.5f, transitionDuraiton).From(0f));
            appearSequence.Join(gridMat.DOColor(_gridColor, transitionDuraiton).From(0f));
            appearSequence.Join(gridMat.DOFloat(6f, Fade, transitionDuraiton).From(24f));
            appearSequence.Join(grid.transform.DOScaleY(1800, transitionDuraiton).From(55f));
            appearSequence.Join(sceneText.DOFade(1f, transitionDuraiton).From(0f));
            appearSequence.SetManual(gameObject);
            
            // TODO: next scene --- call scene manager change grid material color to that nice shade of blue <3
        }

        private void Appear() {
            if (OSCManager.Instance != null) OSCManager.Instance.DisableCore();
            if (AudioMusicManager.Instance != null) {
                AudioMusicManager.Instance.SwitchMusic(sceneMusic);
                AudioMusicManager.Instance.FadeMusic(transitionDuraiton*2, 0.7f);
            }
            player.GetComponent<CoreColor>().RevealColor();
            appearSequence.PlayForward();
        }

        private void Hide() {
            appearSequence.timeScale = 3f;
            if (AudioMusicManager.Instance != null) AudioMusicManager.Instance.FadeMusic(transitionDuraiton, 0f);
            appearSequence.PlayBackwards();
        }
        
        // private void HandleSceneEnd() {
        //     Hide();
        //     StartCoroutine(GoToNextScene());
        // }
        //
        // IEnumerator GoToNextScene() {
        //     yield return new WaitForSeconds(transitionDuraiton/2f);
        //     // GameSceneManager.Instance.LoadScene(SceneEnum.RevealScene);
        // }
        
    }
}
