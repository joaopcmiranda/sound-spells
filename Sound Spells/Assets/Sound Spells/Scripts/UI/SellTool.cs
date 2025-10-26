using Sound_Spells.Systems.Plant;
using UnityEngine;

namespace Sound_Spells.UI
{
    public class SellTool : GardenToolBase
    {
        public override void OnPlotClicked(PlantPlot plot)
        {
            if (!plot.HasPlant)
            {
                Debug.Log("No plant to sell from in this plot.");
                return;
            }

            bool sold = plot.SellFruits();
            if (sold)
            {
                Debug.Log($"Sold fruits from plot: {plot.gameObject.name}");
            }
            else
            {
                Debug.Log("Cannot sell - plant must be blooming and healthy.");
            }
        }
    }
}
