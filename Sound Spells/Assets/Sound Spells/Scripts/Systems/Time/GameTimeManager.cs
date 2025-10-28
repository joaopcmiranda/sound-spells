using System;
using UnityEngine;
using SoundSpells.Systems.Currency;

namespace SoundSpells.Systems.Time
{
    public class GameTimeManager : MonoBehaviour
    {
        public static GameTimeManager Instance { get; private set; }

        [SerializeField] private float secondsPerDay = 60f; // 1 minute = 1 day
        [SerializeField] private float dailyInterestAmount = 1f;

        private float _dayTimer;
        private int _currentDay;

        public int CurrentDay => _currentDay;
        public float DayProgress => _dayTimer / secondsPerDay; // 0 to 1

        public event Action<int> OnNewDay;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _currentDay = 1;
            _dayTimer = 0f;
        }

        private void Update()
        {
            _dayTimer += UnityEngine.Time.deltaTime;

            if (_dayTimer >= secondsPerDay)
            {
                AdvanceDay();
            }
        }

        private void AdvanceDay()
        {
            _dayTimer -= secondsPerDay;
            _currentDay++;

            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddDailyInterest(dailyInterestAmount);
            }

            OnNewDay?.Invoke(_currentDay);
        }
        public float GetTimeRemainingInDay()
        {
            return secondsPerDay - _dayTimer;
        }
    }
}
