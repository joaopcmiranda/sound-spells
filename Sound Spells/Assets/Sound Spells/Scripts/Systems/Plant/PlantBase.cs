using System.Linq;
using Sound_Spells.Models.Plant;
using Sound_Spells.Systems.Weather;
using UnityEngine;

namespace Sound_Spells.Systems.Plant
{
    public class PlantBase : MonoBehaviour
    {
        [SerializeField]
        private PlantData _plantData;

        [SerializeField]
        private PlantState _currentState = PlantState.Healthy;
        [SerializeField]
        private PlantAge _currentAge = PlantAge.Seeded;

        [SerializeField]
        private float _health;

        [SerializeField]
        private float _waterLevel;
        [SerializeField]
        private float _sunlightLevel;
        
        [SerializeField]
        private bool _tooLittleWater;
        [SerializeField]
        private bool _tooMuchWater;
        
        [SerializeField]
        private bool _tooLittleSunlight;
        [SerializeField]
        private bool _tooMuchSunlight;

        [SerializeField]
        private float _growthProgress;
        [SerializeField]
        private float _bloomProgress;

        private SpriteRenderer _spriteRenderer;

        private WeatherSystem _weatherSystem;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _weatherSystem = FindAnyObjectByType<WeatherSystem>();
            if (_plantData)
            {
                Initialize(_plantData);
            }
        }
        
        public void Initialize(PlantData plantData)
        {
            _plantData = plantData;
            _health = plantData.maxHealth;
            _waterLevel = plantData.waterRequirement;
            _sunlightLevel = plantData.sunlightRequirement;
            _growthProgress = 0;
            _bloomProgress = 0;
            _currentState = PlantState.Healthy;
            _currentAge = PlantAge.Seeded;
            UpdateVisuals();
        }

        private void Update()
        {
            UpdateStats();
            UpdateHealth();
            UpdateState();
            UpdateGrowth();
            UpdateVisuals();
            
            // Debug info
            _tooLittleWater = _waterLevel < _plantData.waterRequirement * 0.5f;
            _tooMuchWater = _waterLevel > Mathf.Min(_plantData.waterRequirement * 1.5f, 1f);
            _tooLittleSunlight = _sunlightLevel < _plantData.sunlightRequirement * 0.5f;
            _tooMuchSunlight = _sunlightLevel > Mathf.Min(_plantData.sunlightRequirement * 1.5f, 1f);
        }

        private void UpdateStats()
        {
            var currentWeather = _weatherSystem.currentWeather;
            switch (currentWeather)
            {
                case WeatherType.Sunny:
                    _sunlightLevel += Time.deltaTime * _plantData.sunlightToleranceRate;
                    _waterLevel -= Time.deltaTime * 0.05f; // Evaporation
                    break;
                case WeatherType.Rainy:
                    _waterLevel += Time.deltaTime * _plantData.waterAbsorptionRate * 2f; // Rain absorption
                    _sunlightLevel -= Time.deltaTime * 0.05f; // Less sunlight
                    break;
                case WeatherType.Cloudy:
                    _sunlightLevel += Time.deltaTime * _plantData.sunlightToleranceRate * 0.5f; // Some sunlight
                    _waterLevel -= Time.deltaTime * 0.02f; // Slight evaporation
                    break;
                case WeatherType.Stormy:
                    _waterLevel += Time.deltaTime * _plantData.waterAbsorptionRate * 3f; // Heavy rain
                    _sunlightLevel -= Time.deltaTime * 0.1f; // No sunlight
                    break;
            }

            // Clamp gauges to 0-1 range
            _waterLevel = Mathf.Clamp01(_waterLevel);
            _sunlightLevel = Mathf.Clamp01(_sunlightLevel);
        }

        private void UpdateHealth()
        {
            float healthDelta = 0f;

            // Water impact - check distance from optimal level
            float waterDistance = Mathf.Abs(_waterLevel - _plantData.waterRequirement);
            if (_waterLevel < _plantData.waterRequirement * 0.5f)
            {
                healthDelta -= Time.deltaTime * _plantData.draughtTolerance; // Too little water
            }
            else if (_waterLevel > Mathf.Min(_plantData.waterRequirement * 1.5f, 1f))
            {
                healthDelta -= Time.deltaTime * _plantData.overwaterTolerance; // Overwatering
            }
            else
            {
                healthDelta += Time.deltaTime * 1f; // Optimal water range
            }

            // Sunlight impact - check distance from optimal level
            float sunlightDistance = Mathf.Abs(_sunlightLevel - _plantData.sunlightRequirement);
            if (_sunlightLevel < _plantData.sunlightRequirement * 0.5f)
            {
                healthDelta -= Time.deltaTime * _plantData.lowSunlightTolerance; // Too little sunlight
            }
            else if (_sunlightLevel > Mathf.Min(_plantData.sunlightRequirement * 1.5f, 1f))
            {
                healthDelta -= Time.deltaTime * _plantData.highSunlightTolerance; // Too much sunlight
            }
            else
            {
                healthDelta += Time.deltaTime * 1f; // Optimal sunlight range
            }

            _health += healthDelta;
            _health = Mathf.Clamp(_health, 0, _plantData.maxHealth);
        }

        private void UpdateState()
        {
            if (_health <= 0)
            {
                _currentState = PlantState.Dead;
                _growthProgress = 0;
                _bloomProgress = 0;
            }
            else if (_health < _plantData.maxHealth * 0.5f)
            {
                _currentState = PlantState.Wilted;
                _bloomProgress = 0;
            }
            else if (_currentAge == PlantAge.Mature && _bloomProgress < _plantData.bloomingTime)
            {
                _currentState = PlantState.Healthy;
                _bloomProgress += Time.deltaTime;
            }
            else if (_currentAge == PlantAge.Mature && _bloomProgress >= _plantData.bloomingTime)
            {
                _currentState = PlantState.Blooming;
            }
            else
            {
                _currentState = PlantState.Healthy;
            }
        }

        private void UpdateGrowth()
        {
            if (_currentState is PlantState.Healthy or PlantState.Blooming)
            {
                _growthProgress += Time.deltaTime * _plantData.growthRate;

                _currentAge = _currentAge switch
                {
                    // 10% of growth duration for seeded to seedling
                    PlantAge.Seeded when _growthProgress >= _plantData.growthDuration * 0.1f => PlantAge.Seedling,
                    // 50% of growth duration for seedling to young
                    PlantAge.Seedling when _growthProgress >= _plantData.growthDuration * 0.5f => PlantAge.Young,
                    // 100% of growth duration for young to mature
                    PlantAge.Young when _growthProgress >= _plantData.growthDuration => PlantAge.Mature,
                    _ => _currentAge
                };
            }
        }

        private void UpdateVisuals()
        {
            var plantStateSprite = _plantData.sprites
                .FirstOrDefault(s => s.age == _currentAge && s.state == _currentState);

            if (plantStateSprite.sprite)
            {
                _spriteRenderer.sprite = plantStateSprite.sprite;
            }
        }
    }
}
