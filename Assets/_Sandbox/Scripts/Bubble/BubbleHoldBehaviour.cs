using System;
using _Sandbox.Scripts.Hand;
using _Sandbox.Scripts.Managers;
using UnityEngine;

namespace _Sandbox.Scripts.Bubble
{
    public class BubbleHoldEffect : MonoBehaviour
    {
        [SerializeField] private GameObject overlay;
        [SerializeField] private float maxTime = 4f;
        [SerializeField] private TutorialPhaseManager phaseManager;
        
        private HandController activeHand;
        private float closedHandTimer = 0;
        private bool isTriggered = false;
        private float maxScale = 3.4f;

        private void Update() {
            SetFillOverlay();
            if (closedHandTimer > maxTime) {
                TriggerClosedTime();
            }
        }

        private void TriggerClosedTime() {
            if (isTriggered) return;
            isTriggered = true;
            phaseManager.HandlePhaseZero(true);
        }

        private void SetFillOverlay() {
            float ratio = closedHandTimer / maxTime;
            float scale = Mathf.Lerp(0, maxScale, ratio);
            overlay.transform.localScale = new Vector3(scale, overlay.transform.localScale.y, scale);
            if (activeHand == null) return;
            if (activeHand.IsClosed) {
                closedHandTimer += Time.deltaTime;
            } else {
                closedHandTimer -= Time.deltaTime;
                closedHandTimer = Math.Max(0, closedHandTimer);
            }
        }

        private void OnTriggerEnter(Collider other) {
            var hand = other.GetComponent<HandController>();
            if (hand != null && activeHand == null) {
                activeHand = hand;
            }
        }

        private void OnTriggerExit(Collider other) {
            var hand = other.GetComponent<HandController>();
            if (hand != null && hand == activeHand) {
                activeHand = null;
                closedHandTimer = 0;
                isTriggered = false;
                phaseManager.HandlePhaseZero(false);
            }
        }

    }
}