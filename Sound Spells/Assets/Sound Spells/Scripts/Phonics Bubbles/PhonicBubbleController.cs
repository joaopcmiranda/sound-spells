using System;
using System.Collections.Generic; // Required for Dictionary
using UnityEngine;
using UnityEngine.UIElements;

public class PhonicBubbleController
{
    private static Dictionary<Sprite, Texture2D> pixelatedTextureCache = new Dictionary<Sprite, Texture2D>();
    private const int TARGET_PIXEL_RESOLUTION = 64;

    public VisualElement Root { get; private set; }
    public PhonicData Data { get; private set; }
    
    private Vector2 moveDirection;
    private float moveSpeed;
    public event Action<PhonicBubbleController> OnDismissed;

    public PhonicBubbleController(VisualTreeAsset visualTree, PhonicData data)
    {
        this.Data = data;
        Root = visualTree.CloneTree();
        
        VisualElement bubbleBackground = Root.Q<VisualElement>("BubbleBackground");
        Label phonicLabel = Root.Q<Label>("Phonic");
     
        phonicLabel.text = Data.phonicText;
        
        if (Data.displayImage != null)
        {
            Texture2D textureToDisplay;

            // 1. Check if we have already created a pixelated version of this sprite.
            if (pixelatedTextureCache.ContainsKey(Data.displayImage))
            {
                // 2. If yes, retrieve it from the cache instantly.
                textureToDisplay = pixelatedTextureCache[Data.displayImage];
            }
            else
            {
                // 3. If no, create it now using our utility function.
                Debug.Log($"Generating new pixelated texture for {Data.displayImage.name}...");
                // We pass the sprite's underlying texture and our desired final resolution.
                textureToDisplay = TextureUtility.CreatePixelatedTexture(Data.displayImage, TARGET_PIXEL_RESOLUTION);
                
                // 4. Add the newly created texture to the cache for next time.
                pixelatedTextureCache.Add(Data.displayImage, textureToDisplay);
            }

            // 5. Apply the final, low-resolution texture to the UI.
            bubbleBackground.style.backgroundImage = new StyleBackground(textureToDisplay);
        }
        
        // Initialize movement
        moveDirection = UnityEngine.Random.insideUnitCircle.normalized;
        moveSpeed = UnityEngine.Random.Range(5f, 10f);
    }
    
    public void UpdatePosition(Rect bounds)
    {
        float currentX = Root.style.left.value.value;
        float currentY = Root.style.top.value.value;
        Vector2 delta = moveDirection * moveSpeed * Time.deltaTime;
        float newX = currentX + delta.x;
        float newY = currentY + delta.y;
        if (newX < bounds.xMin || newX > bounds.xMax)
        {
            moveDirection.x *= -1;
            newX = Mathf.Clamp(newX, bounds.xMin, bounds.xMax);
        }
        if (newY < bounds.yMin || newY > bounds.yMax)
        {
            moveDirection.y *= -1;
            newY = Mathf.Clamp(newY, bounds.yMin, bounds.yMax);
        }
        Root.style.left = newX;
        Root.style.top = newY;
    }
    
    public void Dismiss()
    {
        Root.RemoveFromHierarchy();
        OnDismissed?.Invoke(this);
    }
}