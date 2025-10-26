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

        public static bool InputEnabled { get; set; } = true;

        private static readonly Color ValidColor = new Color(0f, 1f, 0f, 0.4f);
        private static readonly Color InvalidColor = new Color(1f, 0f, 0f, 0.4f);
        private static readonly Color NeutralColor = new Color(0.7f, 0.7f, 0.7f, 0.4f);

        private void Awake()
        {
            _plantPlot = GetComponent<PlantPlot>();

            var highlightObj = new GameObject("HoverHighlight");
            highlightObj.transform.SetParent(transform);
            highlightObj.transform.localPosition = Vector3.zero;
            highlightObj.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

            _hoverHighlight = highlightObj.AddComponent<SpriteRenderer>();
            _hoverHighlight.sprite = CreateSquareSprite();
            _hoverHighlight.sortingOrder = 99;
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
            if (!InputEnabled) return;

            HandleHover();

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseClick();
            }
        }

        private void HandleHover()
        {
            if (_gardenToolManager.CurrentTool == GardenTool.None)
            {
                _hoverHighlight.enabled = false;
                return;
            }

            if (Mouse.current == null) return;

            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
                    return _plantPlot.HasPlant ? InvalidColor : NeutralColor;

                case GardenTool.Plant:
                    return !_plantPlot.HasPlant ? ValidColor : NeutralColor;

                case GardenTool.Sell:
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
            if (Mouse.current == null) return;

            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                _gardenToolManager.OnPlotClicked(_plantPlot);
            }
        }

        private Sprite CreateSquareSprite()
        {
            int size = 100;
            var texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;

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
                100f
            );
        }
    }
}
