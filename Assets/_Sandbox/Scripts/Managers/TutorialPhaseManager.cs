using System;
using System.Collections.Generic;
using _Sandbox.Scripts.Bubble;
using _Sandbox.Scripts.UI;
using UnityEngine;

namespace _Sandbox.Scripts.Managers
{
    public class TutorialPhaseManager : MonoBehaviour
    {
        [Header("Phase Zero")]
        [SerializeField] private BubbleEffect leftBubble;
        [SerializeField] private BubbleEffect rightBubble;
        [SerializeField] private TutorialUI tutorialUI;
        
        [Header("Phase One")]
        [SerializeField] private List<BubbleBehavior> phaseOneBubbles = new();

        private int phaseZeroLevel = 0;
        private int phaseOneLevel = 0;

        public void HandlePhaseZero(bool isIncrease) {
            phaseZeroLevel = phaseZeroLevel + (isIncrease ? 1 : -1);
            if (phaseZeroLevel == 2) {
                leftBubble.HideAndDestory();
                rightBubble.HideAndDestory();
                tutorialUI.gameObject.SetActive(false);
                HandlePhaseOne();
            }
        }

        private void HandlePhaseOne() {
            if (phaseOneLevel >= phaseOneBubbles.Count) {
                CalibrationSceneManager.OnEndSceneAction?.Invoke();
                return;
            }
            var curBubble = phaseOneBubbles[phaseOneLevel];
            curBubble.EnableAndAppear();
            phaseOneLevel++;
        }
        
        private void HandleCollectBubble(BubbleBehavior bubble) {
            bubble.CollectAndHide();
            HandlePhaseOne();
        }

        private void OnEnable() {
            BubbleBehavior.OnCollected += HandleCollectBubble;
        }

        private void OnDisable() {
            BubbleBehavior.OnCollected -= HandleCollectBubble;
        }
    }
}