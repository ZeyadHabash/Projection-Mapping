using _Sandbox.Scripts.Utilities.Bases;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Sandbox.Scripts.Managers
{
    public class PauseManager : Singleton<PauseManager>
    {
        private GameObject childPanel;
        private bool isPaused = false;
        
        protected override void Awake() {
            base.Awake();
            childPanel = transform.GetChild(0).gameObject;
            childPanel.SetActive(false);
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.pKey.wasPressedThisFrame) TogglePause();
        }


        private void TogglePause() {
            isPaused = !isPaused;
            if (isPaused) Pause();
            else UnPause();
        }

        private void UnPause() {
            Time.timeScale = 1f;
            AudioListener.pause = false; 
            childPanel.SetActive(false);
        }

        private void Pause() {
            Time.timeScale = 0f;
            AudioListener.pause = true; 
            childPanel.SetActive(true);
        }
    }
}
