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

            _grapesButton = _popupContainer.Q<VisualElement>("GrapesButton");
            _cucumberButton = _popupContainer.Q<VisualElement>("CucumberButton");
            _pineappleButton = _popupContainer.Q<VisualElement>("PineappleButton");

            if (_grapesButton == null || _cucumberButton == null || _pineappleButton == null)
            {
                Debug.LogError("Plant selection buttons not found in UXML.", this);
                return;
            }

            _grapesButton.pickingMode = PickingMode.Position;
            _cucumberButton.pickingMode = PickingMode.Position;
            _pineappleButton.pickingMode = PickingMode.Position;
            _popupContainer.pickingMode = PickingMode.Position;

            _popupContainer.RegisterCallback<ClickEvent>(evt =>
            {
                if (_grapesButton.ContainsPoint(_grapesButton.WorldToLocal(evt.position)))
                {
                    SelectPlant(_plantTool.GetGrapesPlantData());
                    evt.StopPropagation();
                }
                else if (_pineappleButton.ContainsPoint(_pineappleButton.WorldToLocal(evt.position)))
                {
                    SelectPlant(_plantTool.GetPineapplePlantData());
                    evt.StopPropagation();
                }
                else if (_cucumberButton.ContainsPoint(_cucumberButton.WorldToLocal(evt.position)))
                {
                    SelectPlant(_plantTool.GetCucumberPlantData());
                    evt.StopPropagation();
                }
            });

            Hide();
        }

        private void OnDisable()
        {
        }

        public void Show()
        {
            if (_popupContainer != null)
            {
                _popupContainer.style.display = DisplayStyle.Flex;
                PlantPlotInput.InputEnabled = false;
            }
        }

        public void Hide()
        {
            if (_popupContainer != null)
            {
                _popupContainer.style.display = DisplayStyle.None;
                PlantPlotInput.InputEnabled = true;
            }
        }

        public bool IsVisible()
        {
            return _popupContainer != null && _popupContainer.style.display == DisplayStyle.Flex;
        }

        private void SelectPlant(PlantData plantData)
        {
            if (plantData == null)
            {
                Debug.LogError("Selected plant data is null. Make sure PlantData assets are assigned in PlantTool.", this);
                return;
            }

            _plantTool.SetSelectedPlant(plantData);
            OnPlantSelected?.Invoke(plantData);
            Hide();
        }
    }
}
