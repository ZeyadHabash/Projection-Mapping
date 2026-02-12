using System.Collections;
using _Sandbox.Scripts.Enums;
using _Sandbox.ScriptsMariam;
using UnityEngine;

namespace _Sandbox.Scripts.Managers
{
    public class CutsceneSceneManager : MonoBehaviour
    {
        [SerializeField] private float sceneDuration = 30f;
        
        private void Start() {
            StartCoroutine(SceneTimerCoroutine());
        }
        
        private IEnumerator SceneTimerCoroutine() {
            yield return new WaitForSeconds(sceneDuration);
            GameSceneManager.Instance.LoadScene(SceneEnum.CalibrationScene);
        }
    }
}
