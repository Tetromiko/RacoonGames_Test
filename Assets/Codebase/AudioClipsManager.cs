using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Codebase
{
    public class AudioClipsManager : MonoBehaviour
    {
        private const int PoolDefaultCapacity = 10;
        private const int PoolMaxSize = 20;

        [SerializeField] private List<AudioClipsCollection> audioClipsCollections;
        [SerializeField] private List<AudioClip> sounds;

        private readonly List<AudioSource> _activeSources = new();
        private readonly Dictionary<AudioSource, Coroutine> _releaseCoroutines = new();
        private ObjectPool<AudioSource> _sourcesPool;

        public void Initialize()
        {
            _sourcesPool = new ObjectPool<AudioSource>(
                CreateSource,
                source => source.gameObject.SetActive(true),
                source => source.gameObject.SetActive(false),
                source => Destroy(source.gameObject),
                true,
                PoolDefaultCapacity,
                PoolMaxSize
            );

            PreloadAllClips();
        }

        public void PlayRandomFromCollection(string collectionName)
        {
            var collection = audioClipsCollections.FirstOrDefault(c => c.name == collectionName)
                             ?? throw new ArgumentException($"No collection with name ({collectionName})",
                                 nameof(collectionName));

            PlayClip(collection.GetRandom());
        }

        public void PlaySound(string soundName, float pitch = 1f, float volume = 1f, bool loop = false)
        {
            var clip = sounds.FirstOrDefault(s => s.name == soundName)
                       ?? throw new ArgumentException($"No sound with name ({soundName})", nameof(soundName));

            PlayClip(clip, pitch, volume, loop);
        }

        public void StopAllSounds()
        {
            foreach (var coroutine in _releaseCoroutines.Values)
                StopCoroutine(coroutine);

            _releaseCoroutines.Clear();

            var sourcesToRelease = new List<AudioSource>(_activeSources);
            _activeSources.Clear();

            foreach (var source in sourcesToRelease)
            {
                source.Stop();
                _sourcesPool.Release(source);
            }
        }

        private void PlayClip(AudioClip clip, float pitch = 1f, float volume = 1f, bool loop = false)
        {
            var source = _sourcesPool.Get();

            source.clip = clip;
            source.pitch = pitch;
            source.volume = volume;
            source.loop = loop;
            source.Play();

            _activeSources.Add(source);

            if (!loop)
            {
                var duration = clip.length / Mathf.Abs(pitch);
                _releaseCoroutines[source] = StartCoroutine(ReleaseWhenDone(source, duration));
            }
        }

        private IEnumerator ReleaseWhenDone(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);

            _releaseCoroutines.Remove(source);

            if (_activeSources.Remove(source) && source != null)
                _sourcesPool.Release(source);
        }

        private void PreloadAllClips()
        {
            foreach (var clip in sounds)
                PreloadClip(clip);

            foreach (var collection in audioClipsCollections)
                foreach (var clip in collection.Clips)
                    PreloadClip(clip);
        }

        private void PreloadClip(AudioClip clip)
        {
            var source = _sourcesPool.Get();
            source.clip = clip;
            source.volume = 0f;
            source.Play();
            source.Stop();
            _sourcesPool.Release(source);
        }

        private AudioSource CreateSource()
        {
            var go = new GameObject("PooledAudioSource");
            go.transform.SetParent(transform);
            go.SetActive(false);
            return go.AddComponent<AudioSource>();
        }
    }
}