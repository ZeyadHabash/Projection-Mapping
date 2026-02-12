using _Sandbox.Scripts.Enums;
using _Sandbox.Scripts.Utilities.Bases;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Sandbox.ScriptsMariam
{
    public class GameSceneManager : Singleton<GameSceneManager>
    {

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
