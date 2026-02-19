using UnityEngine;

namespace Codebase
{
    public class CubeAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClipsManager audioClipsManager;

        public void Initialize(AudioClipsManager audioClipsManager)
        {
            this.audioClipsManager = audioClipsManager;
        }

        public void PlayHit()
        {
            var pitch = Random.Range(0.8f, 3.2f);
            var volume = 0.2f;
            audioClipsManager.PlaySound("hit", pitch, volume);
        }

        public void PlayMerge()
        {
            audioClipsManager.PlayRandomFromCollection("Popping");
        }
    }
}