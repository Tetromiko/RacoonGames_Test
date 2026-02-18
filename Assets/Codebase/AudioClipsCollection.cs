using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Codebase
{
    [CreateAssetMenu(menuName = "Game/AudioClipCollection", fileName = "AudioClipCollection", order = 0)]
    public class AudioClipsCollection : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField]
        public List<AudioClip> Clips { get; private set; }
        
        public AudioClip Get(int index)
        {
            if (Clips.Count <= index || index < 0)
                throw new IndexOutOfRangeException();
            return Clips[index];
        }

        public AudioClip Get(string clipName)
        {
            var audioClips = Clips.Where(clip => clip.name == clipName).ToList();
            
            if (!audioClips.Any()) throw new KeyNotFoundException();
            
            return audioClips[0];
        }

        public AudioClip GetRandom()
        {
            if (Clips.Count <= 0) throw new KeyNotFoundException();
            
            return Clips[Random.Range(0, Clips.Count)];
        }
    }
}