using _Sandbox.Scripts.Utilities.Bases;
using UnityEngine;

namespace _Sandbox.Scripts.Managers
{
    [DefaultExecutionOrder(-1)]
    public class AudioMusicManager : Singleton<AudioMusicManager>
    {
        private AudioSource audioSource;
        // private AudioMixer
        // private float musicVolume = 0.5f;
        
        protected override void Awake() {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = 0;
        }

        public void SetVolume(float v) {
            audioSource.volume = v;
        }

        //allow changing volume
        //allow changing current song
        //audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20) //level from 0 to 1
    }
}
