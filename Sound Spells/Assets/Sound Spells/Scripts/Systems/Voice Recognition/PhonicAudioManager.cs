using UnityEngine;
using System.Collections.Generic;

namespace Sound_Spells.Systems.Speech
{
    [System.Serializable]
    public class PhonicAudioPair
    {
        public string word;
        public AudioClip clip;
    }

    [RequireComponent(typeof(AudioSource))]
    public class PhonicAudioManager : MonoBehaviour
    {
        public static PhonicAudioManager Instance { get; private set; }

        [SerializeField] private PhonicAudioPair[] phonicClips;

        private AudioSource _audioSource;
        private Dictionary<string, AudioClip> _phonicDictionary;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _audioSource = GetComponent<AudioSource>();
            _phonicDictionary = new Dictionary<string, AudioClip>();
            
            foreach (var pair in phonicClips)
            {
                if (!string.IsNullOrEmpty(pair.word) && pair.clip != null)
                {
                    _phonicDictionary[pair.word.ToLower()] = pair.clip;
                }
            }
        }

        public void PlayPhonic(string word)
        {
            if (string.IsNullOrEmpty(word)) return;

            if (_phonicDictionary.TryGetValue(word.ToLower(), out AudioClip clipToPlay))
            {
                _audioSource.PlayOneShot(clipToPlay);
            }
        }
    }
}