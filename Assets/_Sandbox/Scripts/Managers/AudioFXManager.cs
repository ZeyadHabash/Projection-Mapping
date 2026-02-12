using _Sandbox.Scripts.Utilities.Bases;
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

        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayFXClip(AudioClip audioClip,  float volumeFactor = 1)
        {
            audioSource.clip = audioClip;
            audioSource.volume = volumeFactor * fxVolume;
            audioSource.Play();
        }

        public void PlayRandomFXClip(AudioClip[] audioClips, float volumeFactor = 1)
        {
            int rand = Random.Range(0, audioClips.Length);
            audioSource.clip = audioClips[rand];
            audioSource.volume = volumeFactor * fxVolume;
            audioSource.Play();
        }

        public void SpawnVfx(GameObject prefab, Vector3 position, float lifetime = 0f, Transform parent = null)
        {
            if (prefab == null)
                return;

            var vfx = Instantiate(prefab, position, Quaternion.identity, parent);
            if (lifetime > 0f)
                Destroy(vfx, lifetime);
        }

        public void SpawnRandomVfx(GameObject[] prefabs, Vector3 position, float lifetime = 0f, Transform parent = null)
        {
            if (prefabs == null || prefabs.Length == 0)
                return;

            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            SpawnVfx(prefab, position, lifetime, parent);
        }

    }
}
