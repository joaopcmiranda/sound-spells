using Sound_Spells.Models.Plant;
using Sound_Spells.Systems.Plant;
using UnityEngine;

namespace Sound_Spells.UI
{
    public class PlantTool : GardenToolBase
    {
        [Header("Available Plants")]
        [SerializeField] private PlantData grapesPlantData;
        [SerializeField] private PlantData cucumberPlantData;
        [SerializeField] private PlantData pineapplePlantData;

        private PlantData _selectedPlantData;

        public PlantData SelectedPlant => _selectedPlantData;

        private void Awake()
        {
            _selectedPlantData = grapesPlantData;
        }

        public void SetSelectedPlant(PlantData plantData)
        {
            if (plantData == null)
            {
                Debug.LogError("Cannot set null plant data.", this);
                return;
            }

            _selectedPlantData = plantData;
        }

        public PlantData GetGrapesPlantData() => grapesPlantData;
        public PlantData GetCucumberPlantData() => cucumberPlantData;
        public PlantData GetPineapplePlantData() => pineapplePlantData;

        public override void OnPlotClicked(PlantPlot plot)
        {
            if (plot.HasPlant) return;

            if (_selectedPlantData == null)
            {
                Debug.LogError("No plant selected in PlantTool. Please select a plant first.", this);
                return;
            }

            plot.SowPlant(_selectedPlantData);
        }
    }
}
