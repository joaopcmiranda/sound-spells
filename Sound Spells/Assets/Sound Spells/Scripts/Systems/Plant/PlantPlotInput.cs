using Sound_Spells.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sound_Spells.Systems.Plant
{
    [RequireComponent(typeof(PlantPlot))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlantPlotInput : MonoBehaviour
    {
        private PlantPlot _plantPlot;
        private GardenToolManager _gardenToolManager;
        private Camera _mainCamera;
        private SpriteRenderer _hoverHighlight;

        private static readonly Color ValidColor = new Color(0f, 1f, 0f, 0.4f); // Semi-transparent green
        private static readonly Color InvalidColor = new Color(1f, 0f, 0f, 0.4f); // Semi-transparent red
        private static readonly Color NeutralColor = new Color(0.7f, 0.7f, 0.7f, 0.4f); // Semi-transparent light gray

        private void Awake()
        {
            _plantPlot = GetComponent<PlantPlot>();

            // Create hover highlight sprite renderer
            var highlightObj = new GameObject("HoverHighlight");
            highlightObj.transform.SetParent(transform);
            highlightObj.transform.localPosition = Vector3.zero;
            highlightObj.transform.localScale = new Vector3(1.2f, 1.2f, 1f); // Slightly larger than plot

            _hoverHighlight = highlightObj.AddComponent<SpriteRenderer>();
            _hoverHighlight.sprite = CreateSquareSprite();
            _hoverHighlight.sortingOrder = 99; // Just below plant layer
            _hoverHighlight.enabled = false;
        }

        private void Start()
        {
            _gardenToolManager = FindAnyObjectByType<GardenToolManager>();
            _mainCamera = Camera.main;

            if (_gardenToolManager == null)
            {
                Debug.LogError("GardenToolManager not found in scene. PlantPlotInput requires GardenToolManager.", this);
            }

            if (_mainCamera == null)
            {
                Debug.LogError("Main camera not found. PlantPlotInput requires a camera tagged 'MainCamera'.", this);
            }
        }

        private void Update()
        {
            if (_gardenToolManager == null || _mainCamera == null) return;

            HandleHover();

            // Check for mouse click using new Input System
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseClick();
            }
        }

        private void HandleHover()
        {
            // Only show hover highlight if a tool is selected
            if (_gardenToolManager.CurrentTool == GardenTool.None)
            {
                _hoverHighlight.enabled = false;
                return;
            }

            // Check if mouse exists
            if (Mouse.current == null) return;

            // Convert mouse position to world position
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Raycast to check if we're hovering over this plot
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                _hoverHighlight.enabled = true;
                _hoverHighlight.color = GetHighlightColor();
            }
            else
            {
                _hoverHighlight.enabled = false;
            }
        }

        private Color GetHighlightColor()
        {
            switch (_gardenToolManager.CurrentTool)
            {
                case GardenTool.Shovel:
                    // Red if has plant (valid to remove), gray if empty
                    return _plantPlot.HasPlant ? InvalidColor : NeutralColor;

                case GardenTool.Plant:
                    // Green if empty (valid to plant), gray if has plant
                    return !_plantPlot.HasPlant ? ValidColor : NeutralColor;

                case GardenTool.Sell:
                    // Green if blooming and can sell, gray otherwise
                    if (_plantPlot.PlantedPlant != null && _plantPlot.PlantedPlant.CanSellFruits())
                    {
                        return ValidColor;
                    }
                    return NeutralColor;

                default:
                    return NeutralColor;
            }
        }

        private void HandleMouseClick()
        {
            // Check if mouse exists
            if (Mouse.current == null) return;

            // Convert mouse position to world position
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Raycast to check if we clicked on this plot
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // We clicked on this plant plot
                ProcessToolAction();
            }
        }

        private void ProcessToolAction()
        {
            _gardenToolManager.OnPlotClicked(_plantPlot);
        }

        private Sprite CreateSquareSprite()
        {
            // Create a 100x100 white texture that we'll tint with colors
            int size = 100;
            var texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point; // Crisp edges

            // Fill entire texture with white
            Color[] pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                100f // Pixels per unit - 100 pixels = 1 unit
            );
        }
    }
}
