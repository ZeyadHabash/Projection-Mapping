using UnityEngine;
// using UnityEngine.Audio;
using Utilities.Bases;

namespace _Sandbox.Scripts.Managers
{
    public class AudioFXManager : Singleton<AudioFXManager>
    {
        private AudioSource audioSource;
        // private AudioMixer
        private float fxVolume = 0.5f;
        
        protected override void Awake() {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayFXClip(AudioClip audioClip, Transform spawnTransform, float volumeFactor = 1) {
            audioSource.clip = audioClip;
            audioSource.volume = volumeFactor * fxVolume;
            audioSource.Play();
        }

        public void PlayRandomFXClip(AudioClip[] audioClips, float volumeFactor = 1) {
            int rand = Random.Range(0, audioClips.Length);
            audioSource.clip = audioClips[rand];
            audioSource.volume = volumeFactor * fxVolume;
            audioSource.Play();
        }
    
    }
}
