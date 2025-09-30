using UnityEngine;
using UnityEngine.Windows.Speech;
using System;

namespace Sound_Spells.Systems.Phonics_Generation
{
    public class PhonicsRecogniser : MonoBehaviour
    {
        public event Action OnWordRecognised;
        
        private KeywordRecognizer _keywordRecognizer;

        public void StartListeningFor(string word)
        {
            if (string.IsNullOrEmpty(word)) return;
            
            StopListening();

            var keywords = new string[] { word };
            _keywordRecognizer = new KeywordRecognizer(keywords, ConfidenceLevel.Low);
            _keywordRecognizer.OnPhraseRecognized += PhraseRecognised;
            _keywordRecognizer.Start();
        }

        public void StopListening()
        {
            if (_keywordRecognizer != null && _keywordRecognizer.IsRunning)
            {
                _keywordRecognizer.Stop();
            }
        }
        
        private void PhraseRecognised(PhraseRecognizedEventArgs args)
        {
            OnWordRecognised?.Invoke();
        }

        private void OnDisable()
        {
            if (_keywordRecognizer != null)
            {
                _keywordRecognizer.OnPhraseRecognized -= PhraseRecognised;
                _keywordRecognizer.Dispose();
                _keywordRecognizer = null;
            }
        }
    }
}