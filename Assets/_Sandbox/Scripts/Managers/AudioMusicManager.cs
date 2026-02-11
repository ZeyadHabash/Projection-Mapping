using UnityEngine;
// using UnityEngine.Audio;
using Utilities.Bases;

namespace _Sandbox.Scripts.Managers
{
    public class AudioMusicManager : Singleton<AudioMusicManager>
    {
        private AudioSource audioSource;
        // private AudioMixer
        private float musicVolume = 0.5f;
        
        protected override void Awake() {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        //allow changing volume
        //allow changing current song
        //audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20) //level from 0 to 1
    }
}
