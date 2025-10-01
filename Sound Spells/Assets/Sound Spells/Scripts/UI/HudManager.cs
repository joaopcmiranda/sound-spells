using Sound_Spells.Systems.Phonics_Generation;
using UnityEngine;
using UnityEngine.UIElements;
using Sound_Spells.Systems.Weather;

namespace Sound_Spells.UI
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private UIDocument hudUIDocument;
        [SerializeField] private WeatherSystem weatherSystem;
        [SerializeField] private PhonicsRecogniser phonicsRecogniser;
        [SerializeField] private PhonicRandomiser phonicRandomiser;
    
        private Button _sunButton;
        private Button _rainButton;
        private Button _cloudButton;
        private Button _stormButton;

        private VisualElement _phonicPopup;
        private Label _phonicText;

        private string _phonicWord;
        private WeatherType _pendingWeatherType;

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
            
            _sunButton.clicked += () => ShowPhonicPopup(WeatherType.Sunny);
            _rainButton.clicked += () => ShowPhonicPopup(WeatherType.Rainy);
            _cloudButton.clicked += () => ShowPhonicPopup(WeatherType.Cloudy);
            _stormButton.clicked += () => ShowPhonicPopup(WeatherType.Stormy);

            if (phonicsRecogniser != null)
            {
                phonicsRecogniser.OnWordRecognised += OnPhonicRecognised;
            }
            
            _phonicPopup.style.display = DisplayStyle.None;
        }
        
        private void OnDisable()
        {
            if (phonicsRecogniser != null)
            {
                phonicsRecogniser.OnWordRecognised -= OnPhonicRecognised;
            }
        }
        
        private void ShowPhonicPopup(WeatherType newWeather)
        {
            _pendingWeatherType = newWeather;
            _phonicWord = phonicRandomiser.GenerateRandomWord(_phonicWord);

            if (string.IsNullOrEmpty(_phonicWord)) return;
                
            _phonicText.text = _phonicWord;
            _phonicPopup.style.display = DisplayStyle.Flex;
         
            // OnPhonicRecognised();  
            phonicsRecogniser.StartListeningFor(_phonicWord);
        }

        private void OnPhonicRecognised()
        {
            _phonicPopup.style.display = DisplayStyle.None;
            phonicsRecogniser.StopListening();
            weatherSystem.SetWeather(_pendingWeatherType);
        }
    }
}