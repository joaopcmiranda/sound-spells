using System;
using UnityEngine;

namespace SoundSpells.Systems.Currency
{
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        [SerializeField] private float startingBalance = 100f;

        private float _currentBalance;

        public float CurrentBalance => _currentBalance;

        // Events for UI updates
        public event Action<float> OnBalanceChanged;
        public event Action<float, Vector3> OnMoneySpent;
        public event Action<float, Vector3> OnMoneyEarned;
        public event Action<float> OnDailyInterest;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _currentBalance = startingBalance;
        }

        private void Start()
        {
            OnBalanceChanged?.Invoke(_currentBalance);
        }

        public bool CanAfford(float amount)
        {
            return _currentBalance >= amount;
        }

        public bool TrySpendMoney(float amount, Vector3 worldPosition)
        {
            if (!CanAfford(amount))
            {
                return false;
            }

            _currentBalance -= amount;
            OnBalanceChanged?.Invoke(_currentBalance);
            OnMoneySpent?.Invoke(amount, worldPosition);

            return true;
        }

        public void EarnMoney(float amount, Vector3 worldPosition)
        {
            _currentBalance += amount;
            OnBalanceChanged?.Invoke(_currentBalance);
            OnMoneyEarned?.Invoke(amount, worldPosition);
        }

        public void AddDailyInterest(float amount)
        {
            _currentBalance += amount;
            OnBalanceChanged?.Invoke(_currentBalance);
            OnDailyInterest?.Invoke(amount);
        }

        public void SetBalance(float amount)
        {
            _currentBalance = amount;
            OnBalanceChanged?.Invoke(_currentBalance);
        }

        public void AddMoney(float amount)
        {
            _currentBalance += amount;
            OnBalanceChanged?.Invoke(_currentBalance);
        }
    }
}
