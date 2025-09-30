using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sound_Spells.Systems.Phonics_Generation
{
    public class PhonicRandomiser : MonoBehaviour
    {
        [SerializeField] private List<string> words;

        public string GenerateRandomWord(string previousWordGenerated)
        {
            if (words == null || words.Count == 0)
            {
                Debug.LogError("The word list is empty in the PhonicRandomiser.", this);
                return string.Empty;
            }

            if (words.Count == 1)
            {
                return words[0];
            }

            string newWord;
            do
            {
                int randomIndex = UnityEngine.Random.Range(0, words.Count);
                newWord = words[randomIndex];
            } while (newWord == previousWordGenerated);
            
            return newWord;
        }
    }
}