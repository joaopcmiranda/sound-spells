using UnityEngine;
using UnityEngine.UIElements;
using Sound_Spells.Systems.Weather;

namespace Sound_Spells.UI
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private UIDocument hudUIDocument;
        [SerializeField] private WeatherSystem weatherSystem;
    
        private Button _sunButton;
        private Button _rainButton;
        private Button _cloudButton;
        private Button _stormButton;

        private VisualElement _phonicPopup;
        private Label _phonicText;
        
        private System.Action _sunClickedAction;
        private System.Action _rainClickedAction;
        private System.Action _cloudClickedAction;
        private System.Action _stormClickedAction;

        private void OnEnable()
        {
            if (hudUIDocument == null) return;
            var root = hudUIDocument.rootVisualElement;
            if (root == null) return;
        
            _sunButton = root.Q<Button>("Sun");
            _rainButton = root.Q<Button>("Rain");
            _cloudButton = root.Q<Button>("Cloud");
            _stormButton = root.Q<Button>("Storm");
            _phonicPopup = root.Q<VisualElement>("PhonicPopup");
            _phonicText = root.Q<Label>("Phonic");
            
            if (_sunButton == null || _rainButton == null || _cloudButton == null || _stormButton == null || _phonicPopup == null || _phonicText == null)
            {
                Debug.LogError("A required UI element could not be found. Check UXML names.", this);
                return;
            }
            
            _sunClickedAction = () => ChangeWeatherAndShowPopup(WeatherType.Sunny);
            _rainClickedAction = () => ChangeWeatherAndShowPopup(WeatherType.Rainy);
            _cloudClickedAction = () => ChangeWeatherAndShowPopup(WeatherType.Cloudy);
            _stormClickedAction = () => ChangeWeatherAndShowPopup(WeatherType.Stormy);
            
            _sunButton.clicked += _sunClickedAction;
            _rainButton.clicked += _rainClickedAction;
            _cloudButton.clicked += _cloudClickedAction;
            _stormButton.clicked += _stormClickedAction;
            
            _phonicPopup.style.display = DisplayStyle.None;
        }
        
        private void OnDisable()
        {
            if (_sunButton != null) _sunButton.clicked -= _sunClickedAction;
            if (_rainButton != null) _rainButton.clicked -= _rainClickedAction;
            if (_cloudButton != null) _cloudButton.clicked -= _cloudClickedAction;
            if (_stormButton != null) _stormButton.clicked -= _stormClickedAction;
        }
        
        private void ChangeWeatherAndShowPopup(WeatherType newWeather)
        {
            if (weatherSystem == null || _phonicText == null || _phonicPopup == null) return;

            weatherSystem.SetWeather(newWeather);
            _phonicText.text = $"{newWeather}";
            _phonicPopup.style.display = DisplayStyle.Flex; 
        }
    }
}