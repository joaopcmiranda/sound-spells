using System;
using UnityEngine;
using DG.Tweening;

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
        [Header("Weather Elements")]
        public ParticleSystem rainParticles;
        public ParticleSystem stormParticles;
        public SpriteRenderer cloudCover;
        public SpriteRenderer thunderEffect;

        [Header("Cloud Settings")]
        [SerializeField] private float _cloudAnimationDuration = 2f;
        [SerializeField] private Ease _cloudEaseType = Ease.InOutQuart;

        [Header("Cloud Alpha Values")]
        [SerializeField] private float _sunnyAlpha;
        [SerializeField] private float _cloudyAlpha = 0.3f;
        [SerializeField] private float _rainyAlpha = 0.6f;
        [SerializeField] private float _stormyAlpha = 0.8f;

        [Header("Thunder Settings")]
        [SerializeField] private float _thunderFlashDuration = 0.15f;
        [SerializeField] private float _thunderFlashIntensity = 0.9f;
        [SerializeField] private float _minThunderInterval = 2f;
        [SerializeField] private float _maxThunderInterval = 6f;

        public WeatherType CurrentWeather { get; private set; }
        public static event Action<WeatherType> OnWeatherChanged;

        private Coroutine _thunderCoroutine;

        private void Start()
        {
            // Initialize particles and cloud alpha
            rainParticles?.Stop();
            stormParticles?.Stop();

            if (cloudCover)
            {
                // Start with transparent clouds
                var color = cloudCover.color;
                color.a = _sunnyAlpha;
                cloudCover.color = color;
            }

            if (thunderEffect)
            {
                // Start with transparent thunder effect
                var color = thunderEffect.color;
                color.a = 0f;
                thunderEffect.color = color;
            }

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

            // Call appropriate weather change method
            switch (CurrentWeather)
            {
                case WeatherType.Sunny:
                    ChangeToSunny();
                    break;
                case WeatherType.Rainy:
                    ChangeToRainy();
                    break;
                case WeatherType.Cloudy:
                    ChangeToCloudy();
                    break;
                case WeatherType.Stormy:
                    ChangeToStormy();
                    break;
            }
        }

        private void ChangeToSunny()
        {
            rainParticles.Stop();
            stormParticles.Stop();
            StopThunder();
            AnimateCloudAlpha(_sunnyAlpha);
        }

        private void ChangeToRainy()
        {
            stormParticles.Stop();
            StopThunder();
            AnimateCloudAlpha(_rainyAlpha, () => rainParticles.Play());
        }

        private void ChangeToCloudy()
        {
            rainParticles.Stop();
            stormParticles.Stop();
            StopThunder();
            AnimateCloudAlpha(_cloudyAlpha);
        }

        private void ChangeToStormy()
        {
            rainParticles.Stop();
            AnimateCloudAlpha(_stormyAlpha, () =>
            {
                stormParticles.Play();
                StartThunder();
            });
        }

        private void AnimateCloudAlpha(float targetAlpha, Action onComplete = null)
        {
            if (!cloudCover) return;

            // Animate the alpha channel of the cloud cover
            cloudCover.DOFade(targetAlpha, _cloudAnimationDuration)
                .SetEase(_cloudEaseType)
                .OnComplete(() => onComplete?.Invoke());
        }

        private void StartThunder()
        {
            if (_thunderCoroutine != null)
                StopCoroutine(_thunderCoroutine);

            _thunderCoroutine = StartCoroutine(ThunderRoutine());
        }

        private void StopThunder()
        {
            if (_thunderCoroutine != null)
            {
                StopCoroutine(_thunderCoroutine);
                _thunderCoroutine = null;
            }

            // Ensure thunder effect is hidden
            if (thunderEffect)
            {
                thunderEffect.DOKill();
                var color = thunderEffect.color;
                color.a = 0f;
                thunderEffect.color = color;
            }
        }

        private System.Collections.IEnumerator ThunderRoutine()
        {
            while (CurrentWeather == WeatherType.Stormy)
            {
                // Wait for random interval
                var waitTime = UnityEngine.Random.Range(_minThunderInterval, _maxThunderInterval);
                yield return new WaitForSeconds(waitTime);

                // Trigger thunder flash
                if (CurrentWeather == WeatherType.Stormy) // Check again in case weather changed
                {
                    TriggerThunderFlash();
                }
            }
        }

        private void TriggerThunderFlash()
        {
            if (!thunderEffect) return;

            // Quick flash: 0 -> intensity -> 0
            var sequence = DOTween.Sequence();
            sequence.Append(thunderEffect.DOFade(_thunderFlashIntensity, _thunderFlashDuration * 0.3f))
                    .Append(thunderEffect.DOFade(0f, _thunderFlashDuration * 0.7f));
        }
    }
}
