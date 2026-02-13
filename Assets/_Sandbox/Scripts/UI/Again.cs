using _Sandbox.Scripts.Bubble;
using _Sandbox.Scripts.Enums;
using _Sandbox.ScriptsMariam;
using UnityEngine;

namespace _Sandbox.Scripts.UI
{
    public class Again : MonoBehaviour
    {
        private void HandleRetry(BubbleBehavior obj) {
            GameSceneManager.Instance.LoadScene(SceneEnum.CalibrationScene);
        }
        
        private void OnEnable() {
            BubbleBehavior.OnCollected += HandleRetry;
        }

        private void OnDisable() {
            BubbleBehavior.OnCollected -= HandleRetry;
        }
    }
}