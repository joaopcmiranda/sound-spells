using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BubbleManager : MonoBehaviour
{
    [Header("UI References")]
    public UIDocument uiDocument;
    public VisualTreeAsset bubbleTemplate;

    [Header("Game Data")]
    public List<PhonicData> allPhonics;

    [Header("Game Settings")]
    public float bubbleLifetime = 10.0f;
    public float spawnInterval = 3.0f;
    public int maxBubbles = 5;

    // --- NEW SETTINGS ---
    [Header("Spawning Behaviour")]
    [Tooltip("The fixed width of a bubble in pixels (must match UXML).")]
    public float bubbleWidth = 200f;
    [Tooltip("The minimum distance between the centers of two bubbles when spawning.")]
    public float minSpawnDistance = 220f; // A bit more than the width for a nice gap
    [Tooltip("How many times to try finding a clear spot before giving up.")]
    public int spawnPlacementAttempts = 20;

    // Internal state
    private VisualElement rootVisualElement;
    public List<PhonicBubbleController> activeBubbles = new List<PhonicBubbleController>();
    
    // Reference to the voice recognizer
    private PhonicsRecogniser phonicsRecognizer;

    void Start()
    {
        rootVisualElement = uiDocument.rootVisualElement;
        phonicsRecognizer = FindObjectOfType<PhonicsRecogniser>();
        if (phonicsRecognizer == null)
        {
            Debug.LogError("PhonicsRecognizer not found in the scene! Please add it.");
            return;
        }
        StartCoroutine(SpawnBubbleRoutine());
    }

    void Update()
    {
        if (activeBubbles.Count == 0) return;
        
        float screenWidth = rootVisualElement.resolvedStyle.width;
        float screenHeight = rootVisualElement.resolvedStyle.height;
        Rect bounds = new Rect(0, 0, screenWidth - bubbleWidth, screenHeight - bubbleWidth);
        
        foreach (var bubble in activeBubbles.ToList())
        {
            bubble.UpdatePosition(bounds);
        }
    }

    private IEnumerator SpawnBubbleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (activeBubbles.Count < maxBubbles)
            {
                SpawnBubble();
            }
        }
    }

    private void SpawnBubble()
    {
        var availablePhonics = allPhonics.Where(p => !activeBubbles.Any(b => b.Data == p)).ToList();
        if (availablePhonics.Count == 0) return;

        PhonicData randomPhonic = availablePhonics[Random.Range(0, availablePhonics.Count)];

        // --- CHANGED LOGIC: Find a valid position *before* creating the bubble ---
        Vector2? spawnPosition = FindValidSpawnPosition();

        if (spawnPosition.HasValue)
        {
            // Position is valid, so let's create and place the bubble
            var newBubble = new PhonicBubbleController(bubbleTemplate, randomPhonic);

            // Apply the validated position
            newBubble.Root.style.left = spawnPosition.Value.x;
            newBubble.Root.style.top = spawnPosition.Value.y;

            // Standard setup
            rootVisualElement.Add(newBubble.Root);
            activeBubbles.Add(newBubble);
            newBubble.OnDismissed += OnBubbleDismissed;
            StartCoroutine(BubbleLifetimeRoutine(newBubble));
            phonicsRecognizer.UpdateKeywords(GetActiveWords());
        }
        else
        {
            // We failed to find a spot after several attempts.
            Debug.LogWarning("Could not find a clear spot to spawn a bubble. Screen may be too full.");
        }
    }

    // --- NEW METHOD ---
    /// <summary>
    /// Tries to find a random position that isn't too close to any existing bubbles.
    /// </summary>
    /// <returns>A valid Vector2 position, or null if no position could be found.</returns>
    private Vector2? FindValidSpawnPosition()
    {
        float screenWidth = rootVisualElement.resolvedStyle.width;
        float screenHeight = rootVisualElement.resolvedStyle.height;

        for (int i = 0; i < spawnPlacementAttempts; i++)
        {
            // 1. Generate a random candidate position on the right side of the screen
            float candidateX = Random.Range(screenWidth / 2f, screenWidth - bubbleWidth);
            float candidateY = Random.Range(0, screenHeight - bubbleWidth);
            Vector2 candidatePosition = new Vector2(candidateX, candidateY);

            // 2. Check if this position is valid
            bool isPositionValid = true;
            foreach (var existingBubble in activeBubbles)
            {
                // Get the position of the existing bubble
                float existingX = existingBubble.Root.style.left.value.value;
                float existingY = existingBubble.Root.style.top.value.value;
                Vector2 existingPosition = new Vector2(existingX, existingY);

                // If the distance is less than our minimum, the spot is invalid
                if (Vector2.Distance(candidatePosition, existingPosition) < minSpawnDistance)
                {
                    isPositionValid = false;
                    break; // No need to check other bubbles, this spot is bad
                }
            }

            // 3. If the position is still valid after checking all bubbles, we've found our spot!
            if (isPositionValid)
            {
                return candidatePosition;
            }
        }

        // 4. If the loop finishes, we failed to find a valid position
        return null;
    }

    private IEnumerator BubbleLifetimeRoutine(PhonicBubbleController bubble)
    {
        yield return new WaitForSeconds(bubbleLifetime);
        if (activeBubbles.Contains(bubble))
        {
            bubble.Dismiss();
        }
    }

    public void OnWordRecognized(string word)
    {
        PhonicBubbleController bubbleToPop = activeBubbles.FirstOrDefault(b => b.Data.targetWord.Equals(word, System.StringComparison.OrdinalIgnoreCase));
        if (bubbleToPop != null)
        {
            bubbleToPop.Dismiss();
        }
    }
    
    private void OnBubbleDismissed(PhonicBubbleController bubble)
    {
        activeBubbles.Remove(bubble);
        phonicsRecognizer.UpdateKeywords(GetActiveWords());
    }
    
    private List<string> GetActiveWords()
    {
        return activeBubbles.Select(b => b.Data.targetWord).ToList();
    }
}