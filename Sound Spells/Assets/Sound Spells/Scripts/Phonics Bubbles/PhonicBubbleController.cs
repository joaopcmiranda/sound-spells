using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PhonicBubbleController
{
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
            bubbleBackground.style.backgroundImage = new StyleBackground(Data.displayImage);
        }
        
        moveDirection = UnityEngine.Random.insideUnitCircle.normalized;
        moveSpeed = UnityEngine.Random.Range(20f, 40f);
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