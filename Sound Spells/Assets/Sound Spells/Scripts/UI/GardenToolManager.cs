using Sound_Spells.Systems.Plant;
using UnityEngine;

namespace Sound_Spells.UI
{
    public enum GardenTool
    {
        None,
        Plant,
        Shovel,
        Sell
    }

    [RequireComponent(typeof(PlantTool))]
    [RequireComponent(typeof(ShovelTool))]
    [RequireComponent(typeof(SellTool))]
    public class GardenToolManager : MonoBehaviour
    {
        [SerializeField] private Texture2D shovelCursor;
        [SerializeField] private Texture2D plantCursor;
        [SerializeField] private Texture2D sellCursor;

        [Header("Cursor Hotspots")]
        [SerializeField] private Vector2 shovelHotspot = new Vector2(5, 25);
        [SerializeField] private Vector2 plantHotspot = new Vector2(16, 16);
        [SerializeField] private Vector2 sellHotspot = new Vector2(16, 16);

        private GardenTool _currentTool = GardenTool.None;
        private PlantTool _plantTool;
        private ShovelTool _shovelTool;
        private SellTool _sellTool;

        public GardenTool CurrentTool => _currentTool;

        private void Awake()
        {
            _plantTool = GetComponent<PlantTool>();
            _shovelTool = GetComponent<ShovelTool>();
            _sellTool = GetComponent<SellTool>();
        }

        private void Start()
        {
            if (shovelCursor == null)
            {
                shovelCursor = Resources.Load<Texture2D>("Sprites/Icons/GardenTools/shovel");
            }
            if (plantCursor == null)
            {
                plantCursor = Resources.Load<Texture2D>("Sprites/Icons/GardenTools/leaves-of-a-plant");
            }
            if (sellCursor == null)
            {
                sellCursor = Resources.Load<Texture2D>("Sprites/Icons/GardenTools/money");
            }
        }

        public void SetTool(GardenTool tool)
        {
            _currentTool = tool;
            UpdateCursor();
        }

        public void ToggleTool(GardenTool tool)
        {
            if (_currentTool == tool)
            {
                SetTool(GardenTool.None);
            }
            else
            {
                SetTool(tool);
            }
        }

        private void UpdateCursor()
        {
            switch (_currentTool)
            {
                case GardenTool.Shovel:
                    if (shovelCursor != null)
                    {
                        Cursor.SetCursor(shovelCursor, shovelHotspot, CursorMode.Auto);
                    }
                    else
                    {
                        Debug.LogWarning("Shovel cursor texture not found. Make sure shovel.png is in Resources/Sprites/Icons/GardenTools/");
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    }
                    break;

                case GardenTool.Plant:
                    if (plantCursor != null && IsValidCursorTexture(plantCursor))
                    {
                        Cursor.SetCursor(plantCursor, plantHotspot, CursorMode.Auto);
                    }
                    else
                    {
                        if (plantCursor != null)
                        {
                            Debug.LogWarning("Plant cursor texture is not in a valid format for cursors. It must be RGBA32, readable, with alphaIsTransparency enabled. Using default cursor instead.");
                        }
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    }
                    break;

                case GardenTool.Sell:
                    if (sellCursor != null)
                    {
                        Cursor.SetCursor(sellCursor, sellHotspot, CursorMode.Auto);
                    }
                    else
                    {
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    }
                    break;

                case GardenTool.None:
                default:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
            }
        }

        private void OnDisable()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public void OnPlotClicked(PlantPlot plot)
        {
            switch (_currentTool)
            {
                case GardenTool.Plant:
                    _plantTool.OnPlotClicked(plot);
                    break;
                case GardenTool.Shovel:
                    _shovelTool.OnPlotClicked(plot);
                    break;
                case GardenTool.Sell:
                    _sellTool.OnPlotClicked(plot);
                    break;
                case GardenTool.None:
                    break;
            }
        }

        private bool IsValidCursorTexture(Texture2D texture)
        {
            if (texture == null) return false;
            return texture.format == TextureFormat.RGBA32 && texture.isReadable;
        }
    }
}
