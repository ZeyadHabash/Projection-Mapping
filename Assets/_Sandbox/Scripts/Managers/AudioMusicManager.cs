using _Sandbox.Scripts.Utilities.Bases;
using DG.Tweening;
using UnityEngine;

namespace _Sandbox.Scripts.Managers
{
    [DefaultExecutionOrder(-1)]
    public class AudioMusicManager : Singleton<AudioMusicManager>
    {
        private AudioSource audioSource;
        // private AudioMixer
        // private float musicVolume = 0.5f;
        private Tween volumeTween;
        
        protected override void Awake() {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        public void SetVolume(float v) {
            audioSource.volume = v;
        }

        public void SwitchMusic(AudioClip clip) {
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void FadeMusic(float duration, float volume) {
            volumeTween?.Kill();
             
            volumeTween = DOTween.To(
                () => audioSource.volume,                               
                x => AudioMusicManager.Instance.SetVolume(x), 
                volume,                                  
                duration                     
            ).SetEase(Ease.OutQuad);
        }

        //allow changing volume
        //allow changing current song
        //audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20) //level from 0 to 1
    }
}
