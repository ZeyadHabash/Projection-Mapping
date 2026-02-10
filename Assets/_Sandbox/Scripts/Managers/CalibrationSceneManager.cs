using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Sandbox.Scripts.Managers
{
    public class CalibrationSceneManager : MonoBehaviour
    {
        private Sequence appearSequence;
        [SerializeField] private MeshRenderer grid;
        [SerializeField] private TextMeshPro welcomeText;
        
        private void Awake() {
            SetupAppearSequence();
        }

        private void SetupAppearSequence() {
            appearSequence = DOTween.Sequence();
        }

        private void Appear() {
            // do appear logic...
            appearSequence.PlayForward();
        }

        private void Hide() {
            
        }
    }
}
