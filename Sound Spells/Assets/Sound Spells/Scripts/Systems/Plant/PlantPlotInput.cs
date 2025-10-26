using Sound_Spells.UI;
using UnityEngine;

namespace Sound_Spells.Systems.Plant
{
    [RequireComponent(typeof(PlantPlot))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlantPlotInput : MonoBehaviour
    {
        private PlantPlot _plantPlot;
        private GardenToolManager _gardenToolManager;
        private Camera _mainCamera;

        private void Awake()
        {
            _plantPlot = GetComponent<PlantPlot>();
        }

        private void Start()
        {
            _gardenToolManager = FindObjectOfType<GardenToolManager>();
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

            // Check for mouse click
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
        }

        private void HandleMouseClick()
        {
            // Convert mouse position to world position
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

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
            switch (_gardenToolManager.CurrentTool)
            {
                case GardenTool.Shovel:
                    if (_plantPlot.HasPlant)
                    {
                        _plantPlot.RemovePlant();
                        Debug.Log($"Removed plant from plot: {gameObject.name}");
                    }
                    else
                    {
                        Debug.Log("No plant to remove in this plot.");
                    }
                    break;

                case GardenTool.Plant:
                    // TODO: Implement plant action
                    Debug.Log("Plant tool clicked on plot - not yet implemented");
                    break;

                case GardenTool.Sell:
                    // TODO: Implement sell action
                    Debug.Log("Sell tool clicked on plot - not yet implemented");
                    break;

                case GardenTool.None:
                    // No tool selected, maybe just inspect the plant?
                    Debug.Log($"Clicked on plot: {gameObject.name}");
                    break;
            }
        }
    }
}
