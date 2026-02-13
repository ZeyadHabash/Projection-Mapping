using _Sandbox.Scripts.Enums;
using _Sandbox.Scripts.Utilities.Bases;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _Sandbox.ScriptsMariam
{
    public class GameSceneManager : Singleton<GameSceneManager>
    {

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.digit1Key.wasPressedThisFrame) LoadScene(SceneEnum.CutScene); // Replace with your actual Enum values
            if (kb.digit2Key.wasPressedThisFrame) LoadScene(SceneEnum.CalibrationScene);
            if (kb.digit3Key.wasPressedThisFrame) LoadScene(SceneEnum.CollectionScene);
            if (kb.digit4Key.wasPressedThisFrame) LoadScene(SceneEnum.ProtectionScene);
            if (kb.digit5Key.wasPressedThisFrame) LoadScene(SceneEnum.RevealScene);

            if (kb.rKey.wasPressedThisFrame) ReloadScene();
            
            if (kb.qKey.wasPressedThisFrame) QuitGame();
        }
        
        public void LoadScene(SceneEnum scene)
        {
            SceneManager.LoadScene((int)scene);
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
