using UnityEngine;
using UnityEngine.UIElements;

namespace Sound_Spells.UI
{
    [RequireComponent(typeof(SpellBookController))]
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private UIDocument hudUIDocument;

        private SpellBookController _spellBookController;
        private VisualElement _plantButton;
        private VisualElement _shovelButton;
        private VisualElement _sellButton;
        private VisualElement _magicWandButton;

        private void Awake()
        {
            _spellBookController = GetComponent<SpellBookController>();
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
            Debug.Log("Plant button clicked");
            // TODO: Implement plant logic
        }

        private void OnShovelButtonClicked(ClickEvent evt)
        {
            Debug.Log("Shovel button clicked");
            // TODO: Implement shovel logic
        }

        private void OnSellButtonClicked(ClickEvent evt)
        {
            Debug.Log("Sell button clicked");
            // TODO: Implement sell logic
        }

        private void OnMagicWandButtonClicked(ClickEvent evt)
        {
            _spellBookController.ToggleOpen();
        }
    }
}