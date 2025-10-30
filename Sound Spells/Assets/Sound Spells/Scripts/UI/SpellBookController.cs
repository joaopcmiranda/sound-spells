using Sound_Spells.Systems.Phonics_Generation;
using Sound_Spells.Systems.Weather;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sound_Spells.UI
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(PhonicsRecogniser))]
    [RequireComponent(typeof(PhonicRandomiser))]
    public class SpellBookController : MonoBehaviour
    {
        [SerializeField] private WeatherSystem weatherSystem;
        [SerializeField] private WandController wandController;

        private UIDocument _hudUIDocument;
        private PhonicsRecogniser _phonicsRecogniser;
        private PhonicRandomiser _phonicRandomiser;

        private Button _sunButton;
        private Button _rainButton;
        private Button _cloudButton;
        private Button _stormButton;

        private VisualElement _spellBookContainer;
        private VisualElement _phonicPopup;
        private Label _phonicText;

        private string _phonicWord;
        private WeatherType _pendingWeatherType;
        private bool _isOpen;
        private HudManager _hudManager;

        private void Awake()
        {
            _hudUIDocument = GetComponent<UIDocument>();
            _phonicsRecogniser = GetComponent<PhonicsRecogniser>();
            _phonicRandomiser = GetComponent<PhonicRandomiser>();
            _hudManager = GetComponent<HudManager>();
        }

        private void OnEnable()
        {
            if (_hudUIDocument == null) return;
            var root = _hudUIDocument.rootVisualElement;
            if (root == null) return;

            _sunButton = root.Q<Button>("Sun");
            _rainButton = root.Q<Button>("Rain");
            _cloudButton = root.Q<Button>("Cloud");
            _stormButton = root.Q<Button>("Storm");
            _spellBookContainer = root.Q<VisualElement>("Spells");
            _phonicPopup = root.Q<VisualElement>("PhonicPopup");
            _phonicText = root.Q<Label>("Phonic");

            if (_sunButton == null || _rainButton == null || _cloudButton == null ||
                _stormButton == null || _phonicPopup == null || _phonicText == null)
            {
                Debug.LogError("A required spell UI element could not be found. Check UXML names.", this);
                return;
            }

            _sunButton.clicked += () => ShowPhonicPopup(WeatherType.Sunny);
            _rainButton.clicked += () => ShowPhonicPopup(WeatherType.Rainy);
            _cloudButton.clicked += () => ShowPhonicPopup(WeatherType.Cloudy);
            _stormButton.clicked += () => ShowPhonicPopup(WeatherType.Stormy);

            if (_phonicsRecogniser != null)
            {
                _phonicsRecogniser.OnWordRecognised += OnPhonicRecognised;
                _phonicsRecogniser.OnListeningTimeout += OnPhonicTimeout;
            }
            
            if (wandController != null)
            {
                wandController.OnAnimationStart += DisableSpellButtons;
                wandController.OnAnimationEnd += EnableSpellButtons;
            }

            _phonicPopup.style.display = DisplayStyle.None;

            // Start closed
            Close();
        }

        private void OnDisable()
        {
            if (_phonicsRecogniser != null)
            {
                _phonicsRecogniser.OnWordRecognised -= OnPhonicRecognised;
                _phonicsRecogniser.OnListeningTimeout -= OnPhonicTimeout;
            }
            
            if (wandController != null)
            {
                wandController.OnAnimationStart -= DisableSpellButtons;
                wandController.OnAnimationEnd -= EnableSpellButtons;
            }
        }
        
        private void DisableSpellButtons()
        {
            _sunButton.SetEnabled(false);
            _rainButton.SetEnabled(false);
            _cloudButton.SetEnabled(false);
            _stormButton.SetEnabled(false);
        }

        private void EnableSpellButtons()
        {
            _sunButton.SetEnabled(true);
            _rainButton.SetEnabled(true);
            _cloudButton.SetEnabled(true);
            _stormButton.SetEnabled(true);
        }

        public void ToggleOpen()
        {
            if (_isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void Open()
        {
            _isOpen = true;
            if (_spellBookContainer != null)
            {
                _spellBookContainer.style.display = DisplayStyle.Flex;
            }
        }

        public void Close()
        {
            _isOpen = false;
            if (_spellBookContainer != null)
            {
                _spellBookContainer.style.display = DisplayStyle.None;
            }
            EnableSpellButtons();
        }

        private void ShowPhonicPopup(WeatherType newWeather)
        {
            DisableSpellButtons();
            
            _pendingWeatherType = newWeather;
            _phonicWord = _phonicRandomiser.GenerateRandomWord(_phonicWord);

            if (string.IsNullOrEmpty(_phonicWord))
            {
                EnableSpellButtons();
                return;
            }

            _phonicText.text = _phonicWord;
            _phonicPopup.style.display = DisplayStyle.Flex;

            _phonicsRecogniser.StartListeningFor(_phonicWord);
        }

        private void OnPhonicRecognised()
        {
            if (_hudManager != null) _hudManager.HideHelpButton();
            
            wandController.CastSpell(_pendingWeatherType);
            _phonicPopup.style.display = DisplayStyle.None;
            _phonicsRecogniser.StopListening();
            weatherSystem.SetWeather(_pendingWeatherType);
        }
        
        private void OnPhonicTimeout(string word)
        {
            if (_hudManager != null)
            {
                _hudManager.ShowHelpButton(word);
            }
        }
    }
}
