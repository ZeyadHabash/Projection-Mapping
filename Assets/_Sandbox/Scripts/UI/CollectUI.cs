using System;
using _Sandbox.Scripts.Bubble;
using TMPro;
using UnityEngine;

namespace _Sandbox.Scripts.UI
{
    public class CollectUI : MonoBehaviour
    {
        private TextMeshPro score;
        private int curScore = 0;

        private void Awake() {
            score = GetComponent<TextMeshPro>();
        }

        private void HandleCollect(BubbleBehavior obj) {
            curScore += 1; 
            score.text = $"Collect\n{curScore}";
        }
        
        private void OnEnable() {
            BubbleBehavior.OnCollected += HandleCollect;
        }

        private void OnDisable() {
            BubbleBehavior.OnCollected -= HandleCollect;
        }
    }
}