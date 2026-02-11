using DG.Tweening;
using UnityEngine;

namespace _Sandbox.Scripts.UI
{
    public class TutorialUI : MonoBehaviour
    {
        [Header("Images References")]
        [SerializeField] private GameObject leftHandOpen;
        [SerializeField] private GameObject leftHandClosed;
        [SerializeField] private GameObject rightHandOpen;
        [SerializeField] private GameObject rightHandClosed;
        
        
        private void Start()
        {
            PlayHandLoop();
        }

        private void PlayHandLoop()
        {
            Sequence handSequence = DOTween.Sequence();
            handSequence.AppendCallback(() => SetHandState(isOpen: true));
            handSequence.AppendInterval(2f);
            handSequence.AppendCallback(() => SetHandState(isOpen: false));
            handSequence.AppendInterval(2f);
            handSequence.SetLoops(-1);
        }
        
        private void SetHandState(bool isOpen)
        {
            leftHandOpen.SetActive(isOpen);
            rightHandOpen.SetActive(isOpen);
            leftHandClosed.SetActive(!isOpen);
            rightHandClosed.SetActive(!isOpen);
        }
    }
}
