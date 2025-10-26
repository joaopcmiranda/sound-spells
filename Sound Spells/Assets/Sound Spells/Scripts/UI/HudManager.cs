using UnityEngine;
using UnityEngine.UIElements;

namespace Sound_Spells.UI
{
    [RequireComponent(typeof(SpellBookController))]
    [RequireComponent(typeof(GardenToolManager))]
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private UIDocument hudUIDocument;

        private SpellBookController _spellBookController;
        private GardenToolManager _gardenToolManager;
        private VisualElement _plantButton;
        private VisualElement _shovelButton;
        private VisualElement _sellButton;
        private VisualElement _magicWandButton;

        private void Awake()
        {
            _spellBookController = GetComponent<SpellBookController>();
            _gardenToolManager = GetComponent<GardenToolManager>();
        }

        private void OnEnable()
        {
            if (hudUIDocument == null) return;
            var root = hudUIDocument.rootVisualElement;
            if (root == null) return;

            // Get garden buttons (they're VisualElements, not Buttons)
            var gardenButtons = root.Q<VisualElement>("GardenButtons");
            if (gardenButtons == null)
            {
                Debug.LogError("GardenButtons container not found in UXML.", this);
                return;
            }

            // Get individual buttons by index (since they don't have names in UXML)
            var buttons = gardenButtons.Children();
            var buttonList = new System.Collections.Generic.List<VisualElement>();
            foreach (var button in buttons)
            {
                buttonList.Add(button);
            }

            if (buttonList.Count < 4)
            {
                Debug.LogError($"Expected 4 garden buttons, found {buttonList.Count}.", this);
                return;
            }

            _plantButton = buttonList[0];
            _shovelButton = buttonList[1];
            _sellButton = buttonList[2];
            _magicWandButton = buttonList[3];

            // Register click events
            _plantButton.RegisterCallback<ClickEvent>(OnPlantButtonClicked);
            _shovelButton.RegisterCallback<ClickEvent>(OnShovelButtonClicked);
            _sellButton.RegisterCallback<ClickEvent>(OnSellButtonClicked);
            _magicWandButton.RegisterCallback<ClickEvent>(OnMagicWandButtonClicked);
        }

        private void OnDisable()
        {
            if (_plantButton != null)
                _plantButton.UnregisterCallback<ClickEvent>(OnPlantButtonClicked);
            if (_shovelButton != null)
                _shovelButton.UnregisterCallback<ClickEvent>(OnShovelButtonClicked);
            if (_sellButton != null)
                _sellButton.UnregisterCallback<ClickEvent>(OnSellButtonClicked);
            if (_magicWandButton != null)
                _magicWandButton.UnregisterCallback<ClickEvent>(OnMagicWandButtonClicked);
        }

        private void OnPlantButtonClicked(ClickEvent evt)
        {
            _gardenToolManager.ToggleTool(GardenTool.Plant);
            UpdateButtonVisuals();
            Debug.Log($"Plant tool toggled. Current tool: {_gardenToolManager.CurrentTool}");
        }

        private void OnShovelButtonClicked(ClickEvent evt)
        {
            _gardenToolManager.ToggleTool(GardenTool.Shovel);
            UpdateButtonVisuals();
            Debug.Log($"Shovel tool toggled. Current tool: {_gardenToolManager.CurrentTool}");
        }

        private void OnSellButtonClicked(ClickEvent evt)
        {
            _gardenToolManager.ToggleTool(GardenTool.Sell);
            UpdateButtonVisuals();
            Debug.Log($"Sell tool toggled. Current tool: {_gardenToolManager.CurrentTool}");
        }

        private void UpdateButtonVisuals()
        {
            // Reset all buttons to default state
            _plantButton.RemoveFromClassList("garden-button-active");
            _shovelButton.RemoveFromClassList("garden-button-active");
            _sellButton.RemoveFromClassList("garden-button-active");

            // Highlight the active button
            switch (_gardenToolManager.CurrentTool)
            {
                case GardenTool.Plant:
                    _plantButton.AddToClassList("garden-button-active");
                    break;
                case GardenTool.Shovel:
                    _shovelButton.AddToClassList("garden-button-active");
                    break;
                case GardenTool.Sell:
                    _sellButton.AddToClassList("garden-button-active");
                    break;
            }
        }

        private void OnMagicWandButtonClicked(ClickEvent evt)
        {
            _spellBookController.ToggleOpen();
        }
    }
}