using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Collections;

namespace Sound_Spells.Systems.Phonics_Generation
{
    public class PhonicsRecogniser : MonoBehaviour
    {
        public event Action OnWordRecognised;
        public event Action<string> OnListeningTimeout;
        
        private KeywordRecognizer _keywordRecognizer;
        private Coroutine _listeningTimerCoroutine;
        private const float HelpTimeout = 10.0f;

        public void StartListeningFor(string word)
        {
            if (string.IsNullOrEmpty(word)) return;
            
            StopListening();

            var keywords = new string[] { word };
            _keywordRecognizer = new KeywordRecognizer(keywords, ConfidenceLevel.Low);
            _keywordRecognizer.OnPhraseRecognized += PhraseRecognised;
            _keywordRecognizer.Start();
            
            _listeningTimerCoroutine = StartCoroutine(ListenTimerCoroutine(word));
        }

        public void StopListening()
        {
            if (_keywordRecognizer != null && _keywordRecognizer.IsRunning)
            {
                _keywordRecognizer.Stop();
            }
            
            if (_listeningTimerCoroutine != null)
            {
                StopCoroutine(_listeningTimerCoroutine);
                _listeningTimerCoroutine = null;
            }
        }
        
        private void PhraseRecognised(PhraseRecognizedEventArgs args)
        {
            OnWordRecognised?.Invoke();
            
            if (_listeningTimerCoroutine != null)
            {
                StopCoroutine(_listeningTimerCoroutine);
                _listeningTimerCoroutine = null;
            }
            OnWordRecognised?.Invoke();
        }
        
        private IEnumerator ListenTimerCoroutine(string word)
        {
            yield return new WaitForSeconds(HelpTimeout);
            OnListeningTimeout?.Invoke(word);
            _listeningTimerCoroutine = null;
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