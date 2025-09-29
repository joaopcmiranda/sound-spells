using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System;

public class PhonicsRecogniser : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> keywordActions = new Dictionary<string, Action>();
    private BubbleManager bubbleManager;

    void Start()
    {
        bubbleManager = FindObjectOfType<BubbleManager>();
        if (bubbleManager == null)
        {
            Debug.LogError("PhonicsRecognizer could not find the BubbleManager!");
        }
    }
    
    public void UpdateKeywords(List<string> newKeywords)
    {
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.OnPhraseRecognized -= OnPhraseRecognized;
            keywordRecognizer.Dispose();
            keywordRecognizer = null;
        }
        keywordActions.Clear();
        
        if (newKeywords == null || newKeywords.Count == 0)
        {
            Debug.Log("No active bubbles. Voice recognizer is paused.");
            return;
        }
        
        foreach (string keyword in newKeywords)
        {
            keywordActions.Add(keyword, () => bubbleManager.OnWordRecognized(keyword));
        }
        
        keywordRecognizer = new KeywordRecognizer(keywordActions.Keys.ToArray(), ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        keywordRecognizer.Start();
        
        Debug.Log("Voice Recognizer updated. Now listening for: " + string.Join(", ", keywordActions.Keys));
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Heard keyword: " + args.text);
        if (keywordActions.TryGetValue(args.text, out Action action))
        {
            action.Invoke();
        }
    }
    
    private void OnDestroy()
    {
        if (keywordRecognizer != null)
        {
            keywordRecognizer.Stop();
            keywordRecognizer.Dispose();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
            {
                Debug.Log("Application regained focus, restarting recognizer.");
                keywordRecognizer.Start();
            }
        }
        else
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                Debug.Log("Application lost focus, stopping recognizer.");
                keywordRecognizer.Stop();
            }
        }
    }
}