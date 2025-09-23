using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Sound_Spells.Voice_Recognition
{
    public class VoiceSpellCaster : MonoBehaviour
    {
        private KeywordRecognizer keywordRecognizer;
        private Dictionary<string, Action> spellKeywords = new Dictionary<string, Action>();
        private SpriteRenderer squareRenderer;

        void Start()
        {
            squareRenderer = GetComponent<SpriteRenderer>();
            
            spellKeywords.Add("fire", CastFireSpell);
            spellKeywords.Add("ice", CastIceSpell);
            spellKeywords.Add("earth", CastEarthSpell);
  
            spellKeywords.Add("normal", () => {
                Debug.Log("Casting Reset Spell!");
                squareRenderer.material.color = Color.white;
            });
        
            keywordRecognizer = new KeywordRecognizer(spellKeywords.Keys.ToArray(), ConfidenceLevel.Low);
            keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        
            keywordRecognizer.Start();
            Debug.Log("Voice Spell Caster is now listening...");
        }

        // This function is called every time the recogniser hears a keyword
        private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            Debug.Log("Heard keyword: " + args.text);
            if (spellKeywords.TryGetValue(args.text, out Action spellAction))
            {
                spellAction.Invoke();
            }
        }

        // --- Spell Effect Functions ---
        private void CastFireSpell()
        {
            Debug.Log("Casting Fire Spell!");
            squareRenderer.material.color = Color.red;
        }

        private void CastIceSpell()
        {
            Debug.Log("Casting Ice Spell!");
            squareRenderer.material.color = Color.cyan;
        }
    
        private void CastEarthSpell()
        {
            Debug.Log("Casting Earth Spell!");
            squareRenderer.material.color = new Color(0.5f, 0.25f, 0.0f);
        }
    
        private void OnDestroy()
        {
            if (keywordRecognizer != null)
            {
                keywordRecognizer.Stop();
                keywordRecognizer.OnPhraseRecognized -= OnPhraseRecognized;
                keywordRecognizer.Dispose();
            }
        }
    }
}