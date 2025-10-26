using Sound_Spells.Models.Plant;
using Sound_Spells.Systems.Plant;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sound_Spells.UI
{
    [RequireComponent(typeof(PlantTool))]
    [RequireComponent(typeof(UIDocument))]
    public class PlantSelectionPopup : MonoBehaviour
    {
        private UIDocument _hudUIDocument;
        private VisualElement _popupContainer;
        private VisualElement _grapesButton;
        private VisualElement _cucumberButton;
        private VisualElement _pineappleButton;
        private PlantTool _plantTool;

        public System.Action<PlantData> OnPlantSelected;

        private void Awake()
        {
            _plantTool = GetComponent<PlantTool>();
            _hudUIDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            if (_hudUIDocument == null) return;
            var root = _hudUIDocument.rootVisualElement;
            if (root == null) return;

            _popupContainer = root.Q<VisualElement>("PlantSelectionPopup");
            if (_popupContainer == null)
            {
                Debug.LogError("PlantSelectionPopup container not found in UXML.", this);
                return;
            }

            // Get plant selection buttons
            _grapesButton = _popupContainer.Q<VisualElement>("GrapesButton");
            _cucumberButton = _popupContainer.Q<VisualElement>("CucumberButton");
            _pineappleButton = _popupContainer.Q<VisualElement>("PineappleButton");

            if (_grapesButton == null || _cucumberButton == null || _pineappleButton == null)
            {
                Debug.LogError("Plant selection buttons not found in UXML.", this);
                return;
            }

            // Enable picking mode so buttons can receive clicks
            _grapesButton.pickingMode = PickingMode.Position;
            _cucumberButton.pickingMode = PickingMode.Position;
            _pineappleButton.pickingMode = PickingMode.Position;
            _popupContainer.pickingMode = PickingMode.Position;

            // Register click event on the container and determine which button was clicked
            _popupContainer.RegisterCallback<ClickEvent>(evt =>
            {
                var clickPosition = evt.localPosition;
                Debug.Log($"Popup container clicked at {clickPosition}");

                if (_grapesButton.ContainsPoint(_grapesButton.WorldToLocal(evt.position)))
                {
                    Debug.Log("Click was on Grapes button!");
                    SelectPlant(_plantTool.GetGrapesPlantData());
                    evt.StopPropagation();
                }
                else if (_pineappleButton.ContainsPoint(_pineappleButton.WorldToLocal(evt.position)))
                {
                    Debug.Log("Click was on Pineapple button!");
                    SelectPlant(_plantTool.GetPineapplePlantData());
                    evt.StopPropagation();
                }
                else if (_cucumberButton.ContainsPoint(_cucumberButton.WorldToLocal(evt.position)))
                {
                    Debug.Log("Click was on Cucumber button!");
                    SelectPlant(_plantTool.GetCucumberPlantData());
                    evt.StopPropagation();
                }
                else
                {
                    Debug.Log("Click was not on any plant button.");
                }
            });

            Debug.Log("Plant selection popup registered for click events.");

            // Hide popup by default
            Hide();
        }

        private void OnDisable()
        {
            // Event is registered on the container, no need to unregister individual buttons
        }

        public void Show()
        {
            if (_popupContainer != null)
            {
                _popupContainer.style.display = DisplayStyle.Flex;
                PlantPlotInput.InputEnabled = false; // Disable plant plot clicks while popup is open
                Debug.Log("Plant selection popup shown. Plant plot input disabled.");
            }
        }

        public void Hide()
        {
            if (_popupContainer != null)
            {
                _popupContainer.style.display = DisplayStyle.None;
                PlantPlotInput.InputEnabled = true; // Re-enable plant plot clicks
                Debug.Log("Plant selection popup hidden. Plant plot input enabled.");
            }
        }

        public bool IsVisible()
        {
            return _popupContainer != null && _popupContainer.style.display == DisplayStyle.Flex;
        }

        private void OnGrapesButtonClicked(ClickEvent evt)
        {
            Debug.Log("Grapes button clicked!");
            SelectPlant(_plantTool.GetGrapesPlantData());
        }

        private void OnCucumberButtonClicked(ClickEvent evt)
        {
            Debug.Log("Cucumber button clicked!");
            SelectPlant(_plantTool.GetCucumberPlantData());
        }

        private void OnPineappleButtonClicked(ClickEvent evt)
        {
            Debug.Log("Pineapple button clicked!");
            SelectPlant(_plantTool.GetPineapplePlantData());
        }

        private void SelectPlant(PlantData plantData)
        {
            if (plantData == null)
            {
                Debug.LogError("Selected plant data is null. Make sure PlantData assets are assigned in PlantTool.", this);
                return;
            }

            Debug.Log($"Selecting plant: {plantData.name}");
            _plantTool.SetSelectedPlant(plantData);

            if (OnPlantSelected != null)
            {
                Debug.Log("Invoking OnPlantSelected event.");
                OnPlantSelected.Invoke(plantData);
            }
            else
            {
                Debug.LogWarning("OnPlantSelected event has no subscribers.");
            }

            Hide();
        }
    }
}
