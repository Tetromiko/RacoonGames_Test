using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Codebase
{
    public class AudioClipsManager : MonoBehaviour
    {
        [SerializeField] private List<AudioClipsCollection> audioClipsCollections;
        [SerializeField] private List<AudioClip> sounds;
        [SerializeField] private int defaultPoolSize = 10;
        [SerializeField] private int maxPoolSize = 20;

        private ObjectPool<AudioSource> _sourcesPool;

        public static AudioClipsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _sourcesPool = new ObjectPool<AudioSource>(
                createFunc: CreateSource,
                actionOnGet: source => source.gameObject.SetActive(true),
                actionOnRelease: source => source.gameObject.SetActive(false),
                actionOnDestroy: source => Destroy(source.gameObject),
                collectionCheck: true,
                defaultCapacity: defaultPoolSize,
                maxSize: maxPoolSize
            );
        }

        private AudioSource CreateSource()
        {
            var go = new GameObject("PooledAudioSource");
            go.transform.SetParent(transform);
            go.SetActive(false);
            return go.AddComponent<AudioSource>();
        }

        public void PlayRandomFromCollection(string collectionName)
        {
            var collection = audioClipsCollections.FirstOrDefault(c => c.name == collectionName);

            if (collection == null)
                throw new Exception($"No collection with name ({collectionName})");

            PlayClip(collection.GetRandom());
        }
        
        public void PlaySound(string soundName, float pitch = 1f, float volume = 1f)
        {
            var clip = sounds.FirstOrDefault(s => s.name == soundName);

            if (clip == null)
                throw new Exception($"No sound with name ({soundName})");

            PlayClip(clip, pitch, volume);
        }

        private void PlayClip(AudioClip clip, float pitch = 1f, float volume = 1f)
        {
            var source = _sourcesPool.Get();
            source.pitch = pitch;
            source.volume = volume;
            source.clip = clip;
            source.Play();
            StartCoroutine(ReleaseWhenDone(source, clip.length / source.pitch));
        }

        private IEnumerator ReleaseWhenDone(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            _sourcesPool.Release(source);
        }
    }
}