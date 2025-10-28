using System.Linq;
using Sound_Spells.Models.Plant;
using Sound_Spells.Systems.Weather;
using SoundSpells.Systems.Currency;
using UnityEngine;

namespace Sound_Spells.Systems.Plant
{
    public class PlantBase : MonoBehaviour
    {
        [SerializeField]
        private PlantData _plantData;

        public ElementIndicator elementIndicator;

        [Header("Global Evaporation Rates")]
        [SerializeField]
        private float _sunnyEvaporationRate = 0.02f; // Water loss rate per second in sunny weather
        [SerializeField]
        private float _cloudyEvaporationRate = 0.01f; // Water loss rate per second in cloudy weather

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
            UpdateIndicator();
        }

        private void UpdateStats()
        {
            var currentWeather = _weatherSystem.CurrentWeather;
            switch (currentWeather)
            {
                case WeatherType.Sunny:
                    _sunlightLevel += Time.deltaTime * _plantData.sunlightToleranceRate;
                    _waterLevel -= Time.deltaTime * _sunnyEvaporationRate; // Evaporation
                    break;
                case WeatherType.Rainy:
                    _waterLevel += Time.deltaTime * _plantData.waterAbsorptionRate * 2f; // Rain absorption
                    _sunlightLevel -= Time.deltaTime * 0.05f; // Less sunlight
                    break;
                case WeatherType.Cloudy:
                    _sunlightLevel += Time.deltaTime * _plantData.sunlightToleranceRate * 0.5f; // Some sunlight
                    _waterLevel -= Time.deltaTime * _cloudyEvaporationRate; // Slight evaporation
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
            var healthDelta = 0f;

            // Water impact - check distance from optimal level
            var waterDistance = Mathf.Abs(_waterLevel - _plantData.waterRequirement);
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
            var sunlightDistance = Mathf.Abs(_sunlightLevel - _plantData.sunlightRequirement);
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
        
        public bool CanSellFruits()
        {
            return _currentState == PlantState.Blooming && _currentAge == PlantAge.Mature;
        }

        public bool SellFruits()
        {
            if (!CanSellFruits())
            {
                return false;
            }

            _bloomProgress = 0;
            _currentState = PlantState.Healthy;
            UpdateVisuals();

            if (CurrencyManager.Instance != null && _plantData != null)
            {
                CurrencyManager.Instance.EarnMoney(_plantData.sellValue, transform.position);
            }

            return true;
        }

        private void UpdateIndicator()
        {
            if (!elementIndicator) return;

            // If plant is dead, show dead indicator
            if (_currentState == PlantState.Dead)
            {
                elementIndicator.Content = ElementIndicator.ContentType.Dead;
                elementIndicator.State = ElementIndicator.IndicatorState.Neutral;
                return;
            }

            var waterDistance = Mathf.Abs(_waterLevel - _plantData.waterRequirement);
            var sunlightDistance = Mathf.Abs(_sunlightLevel - _plantData.sunlightRequirement);

            var showWater = waterDistance >= sunlightDistance;

            if (showWater)
            {
                // Show water indicator based on current water level
                if (_waterLevel < _plantData.waterRequirement * 0.3f)
                {
                    elementIndicator.Content = ElementIndicator.ContentType.Water;
                    elementIndicator.State = ElementIndicator.IndicatorState.HeavyIncrease; // Need water badly
                }
                else if (_waterLevel < _plantData.waterRequirement * 0.6f)
                {
                    elementIndicator.Content = ElementIndicator.ContentType.Water;
                    elementIndicator.State = ElementIndicator.IndicatorState.Increase; // Need some water
                }
                else if (_waterLevel > Mathf.Min(_plantData.waterRequirement * 1.8f, 1f))
                {
                    elementIndicator.Content = ElementIndicator.ContentType.Water;
                    elementIndicator.State = ElementIndicator.IndicatorState.HeavyDecrease; // Need less water
                }
                else if (_waterLevel > Mathf.Min(_plantData.waterRequirement * 1.5f, 1f))
                {
                    elementIndicator.Content = ElementIndicator.ContentType.Water;
                    elementIndicator.State = ElementIndicator.IndicatorState.Decrease; // Need slightly less water
                }
                else
                {
                    // Water is optimal, but check if we should show anything
                    if (sunlightDistance > 0.1f) // Only show sunlight if it's significantly off
                    {
                        showWater = false; // Switch to showing sunlight
                    }
                    else
                    {
                        // Both are optimal
                        elementIndicator.State = ElementIndicator.IndicatorState.None;
                        return;
                    }
                }
            }

            if (!showWater)
            {
                // Show sunlight indicator based on current sunlight level
                if (_sunlightLevel < _plantData.sunlightRequirement * 0.5f)
                {
                    elementIndicator.Content = ElementIndicator.ContentType.Sun;
                    elementIndicator.State = ElementIndicator.IndicatorState.HeavyIncrease; // Need sun badly
                }
                else if (_sunlightLevel < _plantData.sunlightRequirement * 0.6f)
                {
                    elementIndicator.Content = ElementIndicator.ContentType.Sun;
                    elementIndicator.State = ElementIndicator.IndicatorState.Increase; // Need some sun
                }
                else if (_sunlightLevel > Mathf.Min(_plantData.sunlightRequirement * 1.8f, 1f))
                {
                    elementIndicator.Content = ElementIndicator.ContentType.Sun;
                    elementIndicator.State = ElementIndicator.IndicatorState.HeavyDecrease; // Need less sun
                }
                else if (_sunlightLevel > Mathf.Min(_plantData.sunlightRequirement * 1.5f, 1f))
                {
                    elementIndicator.Content = ElementIndicator.ContentType.Sun;
                    elementIndicator.State = ElementIndicator.IndicatorState.Decrease; // Need slightly less sun
                }
                else
                {
                    // Both water and sunlight are optimal
                    elementIndicator.State = ElementIndicator.IndicatorState.None;
                }
            }
        }
    }
}
