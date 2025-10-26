using UnityEngine;
using SoundSpells.Systems.Currency;
using TMPro;

namespace Sound_Spells.UI
{
    public class CurrencyFeedbackManager : MonoBehaviour
    {
        [SerializeField] private GameObject floatingTextPrefab;

        [Header("Colors")]
        [SerializeField] private Color moneySpentColor = new Color(1f, 0.3f, 0.3f); // Red
        [SerializeField] private Color moneyEarnedColor = new Color(0.3f, 1f, 0.3f); // Green
        [SerializeField] private Color dailyInterestColor = new Color(1f, 0.84f, 0f); // Gold

        private bool _isSubscribed = false;

        private void OnEnable()
        {
            TrySubscribeToEvents();
        }

        private void Start()
        {
            if (!_isSubscribed)
            {
                TrySubscribeToEvents();
            }
        }

        private void TrySubscribeToEvents()
        {
            if (_isSubscribed) return;

            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnMoneySpent += OnMoneySpent;
                CurrencyManager.Instance.OnMoneyEarned += OnMoneyEarned;
                CurrencyManager.Instance.OnDailyInterest += OnDailyInterest;
                _isSubscribed = true;
            }
        }

        private void OnDisable()
        {
            if (_isSubscribed && CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnMoneySpent -= OnMoneySpent;
                CurrencyManager.Instance.OnMoneyEarned -= OnMoneyEarned;
                CurrencyManager.Instance.OnDailyInterest -= OnDailyInterest;
                _isSubscribed = false;
            }
        }

        private void OnMoneySpent(float amount, Vector3 worldPosition)
        {
            CreateFloatingText($"-${amount:F0}", worldPosition, moneySpentColor);
        }

        private void OnMoneyEarned(float amount, Vector3 worldPosition)
        {
            CreateFloatingText($"+${amount:F0}", worldPosition, moneyEarnedColor);
        }

        private void OnDailyInterest(float amount)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height * 0.8f, 10f);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenCenter);
            CreateFloatingText($"Daily Interest: +${amount:F0}", worldPosition, dailyInterestColor);
        }

        private void CreateFloatingText(string text, Vector3 worldPosition, Color color)
        {
            if (floatingTextPrefab == null) return;

            GameObject textObject = Instantiate(floatingTextPrefab, worldPosition, Quaternion.identity);

            var floatingText = textObject.GetComponent<FloatingText>();
            if (floatingText != null)
            {
                floatingText.SetText(text);
                floatingText.SetColor(color);
            }
            else
            {
                var textMesh = textObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (textMesh != null)
                {
                    textMesh.text = text;
                    textMesh.color = color;
                }
            }
        }
    }
}
