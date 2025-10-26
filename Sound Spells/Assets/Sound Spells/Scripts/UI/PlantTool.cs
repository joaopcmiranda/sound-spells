using Sound_Spells.Models.Plant;
using Sound_Spells.Systems.Plant;
using UnityEngine;

namespace Sound_Spells.UI
{
    public class PlantTool : GardenToolBase
    {
        [SerializeField] private PlantData grapesPlantData;

        public override void OnPlotClicked(PlantPlot plot)
        {
            if (plot.HasPlant)
            {
                Debug.Log("Cannot plant - plot already has a plant.");
                return;
            }

            if (grapesPlantData == null)
            {
                Debug.LogError("Grapes PlantData not assigned in PlantTool. Please assign it in the inspector.", this);
                return;
            }

            plot.SowPlant(grapesPlantData);
            Debug.Log($"Planted grapes in plot: {plot.gameObject.name}");
        }
    }
}
