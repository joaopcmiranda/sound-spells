using Sound_Spells.Models.Plant;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sound_Spells.UI
{
    [RequireComponent(typeof(SpellBookController))]
    [RequireComponent(typeof(GardenToolManager))]
    [RequireComponent(typeof(PlantSelectionPopup))]
    [RequireComponent(typeof(PlantTool))]
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private UIDocument hudUIDocument;

        [Header("Plant Button Icons")]
        [SerializeField] private Texture2D whitePlantIcon;
        [SerializeField] private Sprite grapesIcon;
        [SerializeField] private Sprite pineappleIcon;
        [SerializeField] private Sprite cucumberIcon;

        private SpellBookController _spellBookController;
        private GardenToolManager _gardenToolManager;
        private PlantSelectionPopup _plantSelectionPopup;
        private PlantTool _plantTool;
        private VisualElement _plantButton;
        private VisualElement _shovelButton;
        private VisualElement _sellButton;
        private VisualElement _magicWandButton;

        private void Awake()
        {
            _spellBookController = GetComponent<SpellBookController>();
            _gardenToolManager = GetComponent<GardenToolManager>();
            _plantSelectionPopup = GetComponent<PlantSelectionPopup>();
            _plantTool = GetComponent<PlantTool>();
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

            // Subscribe to plant selection events
            if (_plantSelectionPopup != null)
            {
                _plantSelectionPopup.OnPlantSelected += OnPlantSelected;
            }
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

            // Unsubscribe from plant selection events
            if (_plantSelectionPopup != null)
            {
                _plantSelectionPopup.OnPlantSelected -= OnPlantSelected;
            }
        }

        private void OnPlantButtonClicked(ClickEvent evt)
        {
            if (_gardenToolManager.CurrentTool == GardenTool.Plant)
            {
                _gardenToolManager.SetTool(GardenTool.None);
                _plantSelectionPopup.Hide();
                ResetPlantButtonIcon();
                UpdateButtonVisuals();
            }
            else if (_plantSelectionPopup.IsVisible())
            {
                _plantSelectionPopup.Hide();
            }
            else
            {
                _plantSelectionPopup.Show();
            }
        }

        private void OnShovelButtonClicked(ClickEvent evt)
        {
            _plantSelectionPopup.Hide();
            ResetPlantButtonIcon();
            _gardenToolManager.ToggleTool(GardenTool.Shovel);
            UpdateButtonVisuals();
        }

        private void OnSellButtonClicked(ClickEvent evt)
        {
            _plantSelectionPopup.Hide();
            ResetPlantButtonIcon();
            _gardenToolManager.ToggleTool(GardenTool.Sell);
            UpdateButtonVisuals();
        }

        private void UpdateButtonVisuals()
        {
            _plantButton.RemoveFromClassList("garden-button-active");
            _shovelButton.RemoveFromClassList("garden-button-active");
            _sellButton.RemoveFromClassList("garden-button-active");

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

        private void OnPlantSelected(PlantData plantData)
        {
            _gardenToolManager.SetTool(GardenTool.Plant);
            UpdateButtonVisuals();
            UpdatePlantButtonIcon(plantData);
        }

        private void UpdatePlantButtonIcon(PlantData plantData)
        {
            if (_plantButton == null || plantData == null) return;

            if (plantData == _plantTool.GetGrapesPlantData() && grapesIcon != null)
            {
                _plantButton.style.backgroundImage = new StyleBackground(Background.FromSprite(grapesIcon));
                SetBackgroundSize(_plantButton, 50, 70);
            }
            else if (plantData == _plantTool.GetPineapplePlantData() && pineappleIcon != null)
            {
                _plantButton.style.backgroundImage = new StyleBackground(Background.FromSprite(pineappleIcon));
                SetBackgroundSize(_plantButton, 73, 67);
            }
            else if (plantData == _plantTool.GetCucumberPlantData() && cucumberIcon != null)
            {
                _plantButton.style.backgroundImage = new StyleBackground(Background.FromSprite(cucumberIcon));
                SetBackgroundSize(_plantButton, 50, 68);
            }
            else if (whitePlantIcon != null)
            {
                _plantButton.style.backgroundImage = new StyleBackground(Background.FromTexture2D(whitePlantIcon));
                ResetBackgroundSize(_plantButton);
            }
        }

        private void ResetPlantButtonIcon()
        {
            if (_plantButton == null || whitePlantIcon == null) return;

            _plantButton.style.backgroundImage = new StyleBackground(Background.FromTexture2D(whitePlantIcon));
            ResetBackgroundSize(_plantButton);
        }

        private void SetBackgroundSize(VisualElement element, float widthPercent, float heightPercent)
        {
            var size = new BackgroundSize(new Length(widthPercent, LengthUnit.Percent), new Length(heightPercent, LengthUnit.Percent));
            element.style.backgroundSize = new StyleBackgroundSize(size);
        }

        private void ResetBackgroundSize(VisualElement element)
        {
            element.style.backgroundSize = StyleKeyword.Null;
        }
    }
}