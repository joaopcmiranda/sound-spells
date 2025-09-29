using Sound_Spells.Models.Plant;
using UnityEngine;
namespace Sound_Spells.Systems.Plant
{
    public class PlantPlot : MonoBehaviour
    {
        public GameObject plantPrefab;
        
        private PlantBase _plantedPlant;

        public void SowPlant(PlantData plant)
        {
            if (_plantedPlant)
            {
                Debug.LogWarning("A plant is already planted in this plot.");
                return;
            }

            var plantObject = Instantiate(plantPrefab, transform.position, Quaternion.identity);
            plantObject.SetActive(true);
            var plantBase = plantObject.GetComponent<PlantBase>();
            plantBase.Initialize(plant);
            _plantedPlant = plantBase;
        }
        
        public void RemovePlant()
        {
            if (_plantedPlant)
            {
                Destroy(_plantedPlant.gameObject);
                _plantedPlant = null;
            }
            else
            {
                Debug.LogWarning("No plant to remove in this plot.");
            }
        }
        
        // Debug 
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.softYellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
        }
    }
}
