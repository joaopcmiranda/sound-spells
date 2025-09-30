using System;
using UnityEngine;

namespace Sound_Spells.Systems.Weather
{
    public enum WeatherType
    {
        Sunny,
        Rainy,
        Cloudy,
        Stormy
    }
    
    public class WeatherSystem : MonoBehaviour
    {
        public WeatherType CurrentWeather { get; private set; }
        public static event Action<WeatherType> OnWeatherChanged;
        
        private void Start()
        {
            SetWeather(WeatherType.Sunny); 
        }
    
        public void SetWeather(WeatherType newWeather)
        {
            if (CurrentWeather == newWeather)
            {
                return;
            }

            CurrentWeather = newWeather;
            Debug.Log($"Weather has changed to: {CurrentWeather}");
            OnWeatherChanged?.Invoke(CurrentWeather);
        }
    }
}