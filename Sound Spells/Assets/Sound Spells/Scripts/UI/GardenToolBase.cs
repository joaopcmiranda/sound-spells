using Sound_Spells.Systems.Plant;
using UnityEngine;

namespace Sound_Spells.UI
{
    public abstract class GardenToolBase : MonoBehaviour
    {
        public abstract void OnPlotClicked(PlantPlot plot);
    }
}
