using Sound_Spells.Systems.Plant;
using UnityEngine;

namespace Sound_Spells.UI
{
    public class ShovelTool : GardenToolBase
    {
        public override void OnPlotClicked(PlantPlot plot)
        {
            if (!plot.HasPlant) return;
            plot.RemovePlant();
        }
    }
}
