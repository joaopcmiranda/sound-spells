using UnityEngine;

public class ElementIndicator : MonoBehaviour
{
    public enum ContentType
    {
        Sun, 
        Water, 
        Dead
    }

    private ContentType _contentType = ContentType.Sun;
    public ContentType Content
    {
        get => _contentType;
        set
        {
            _contentType = value;
            UpdateIndicator();
        }
    }

    public enum IndicatorState
    {
        None,
        Increase,
        HeavyIncrease,
        Decrease,
        HeavyDecrease,
        Neutral
    }

    private IndicatorState _indicatorState = IndicatorState.None;
    public IndicatorState State
    {
        get => _indicatorState;
        set
        {
            _indicatorState = value;
            UpdateIndicator();
        }
    }

    public GameObject arrowUp;
    public GameObject arrowDown;
    public GameObject heavyArrowUp;
    public GameObject heavyArrowDown;

    public Sprite sunSprite;
    public Sprite waterSprite;
    public Sprite deadSprite;
    
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateIndicator();
    }
    
    private void UpdateIndicator()
    {
        // Update sprite based on content type
        _spriteRenderer.sprite = Content switch
        {
            ContentType.Sun => sunSprite,
            ContentType.Water => waterSprite,
            ContentType.Dead => deadSprite,
            _ => _spriteRenderer.sprite
        };

        // Update arrows based on indicator state
        arrowUp.SetActive(State == IndicatorState.Increase);
        heavyArrowUp.SetActive(State == IndicatorState.HeavyIncrease);
        arrowDown.SetActive(State == IndicatorState.Decrease);
        heavyArrowDown.SetActive(State == IndicatorState.HeavyDecrease);

        // If state is Neutral or None, hide all arrows
        if (State is IndicatorState.Neutral or IndicatorState.None)
        {
            arrowUp.SetActive(false);
            heavyArrowUp.SetActive(false);
            arrowDown.SetActive(false);
            heavyArrowDown.SetActive(false);
        }
        
        // If state is None, hide the entire indicator
        gameObject.SetActive(State != IndicatorState.None);
    }
}
