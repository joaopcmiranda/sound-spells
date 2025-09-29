namespace Sound_Spells.Systems.Weather
{
    public enum WeatherType
    {
        Sunny,
        Rainy,
        Cloudy,
        Stormy
    }
    
    public class WeatherSystem : UnityEngine.MonoBehaviour
    {
        public WeatherType currentWeather;
    }
}
