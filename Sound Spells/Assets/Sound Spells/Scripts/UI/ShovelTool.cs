using Sound_Spells.Systems.Plant;
using UnityEngine;

namespace Sound_Spells.UI
{
    public class ShovelTool : GardenToolBase
    {
        public override void OnPlotClicked(PlantPlot plot)
        {
            if (!plot.HasPlant)
            {
                Debug.Log("No plant to remove in this plot.");
                return;
            }

            plot.RemovePlant();
            Debug.Log($"Removed plant from plot: {plot.gameObject.name}");
        }
    }
}
