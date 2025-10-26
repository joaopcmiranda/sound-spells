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

        public GardenTool CurrentTool => _currentTool;

        private void Start()
        {
            // Load cursors from the GardenTools folder
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
                    if (plantCursor != null)
                    {
                        Cursor.SetCursor(plantCursor, plantHotspot, CursorMode.Auto);
                    }
                    else
                    {
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
            // Reset cursor when disabled
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
