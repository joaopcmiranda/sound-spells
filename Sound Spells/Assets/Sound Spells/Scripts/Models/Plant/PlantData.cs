using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sound_Spells.Models.Plant
{
    public enum PlantType
    {
        Flower,
        Tree,
        Bush
    }
    
    [System.Serializable]
    public struct PlantStateSprite
    {
        public PlantAge age;
        public PlantState state;
        public Sprite sprite;
    }

    [CreateAssetMenu(fileName = "Plant", menuName = "SoundSpells/Plant", order = 0)]
    public class PlantData : ScriptableObject
    {
        public string plantName;
        public PlantType type;

        // Stats
        
        public float maxHealth = 100f;
        public float growthDuration = 180f; // Time in seconds to reach maturity
        public float waterRequirement = 0.6f; // Optimal water level (0-1 range)
        public float sunlightRequirement = 0.7f; // Optimal sunlight level (0-1 range)

        // Rates

        public float growthRate = 1f; // Rate of growth per second
        public float waterAbsorptionRate = 0.1f; // How fast the plant absorbs water (0-1 range per second)
        public float sunlightToleranceRate = 0.1f; // How fast the plant sunlight level changes (0-1 range per second)
        public float bloomingTime = 20f; // Time in seconds to bloom when mature
        
        // Tolerances
        
        public float draughtTolerance = 2f; // How much being underwater affects health
        public float overwaterTolerance = 1f; // How much being overwatered affects health
        public float lowSunlightTolerance = .5f; // How much low sunlight affects health
        public float highSunlightTolerance = .8f; // How much too much sunlight affects health
        
        // Sprites 
        
        public PlantStateSprite[] sprites; // Array of sprites for different states and ages
    }

}
