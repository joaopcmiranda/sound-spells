using UnityEngine;
using TMPro;

namespace Sound_Spells.UI
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float lifetime = 1.5f;
        [SerializeField] private float fadeStartTime = 0.5f;

        private TextMeshProUGUI _textMesh;
        private Canvas _canvas;
        private float _timer;
        private Color _startColor;

        private void Awake()
        {
            _textMesh = GetComponent<TextMeshProUGUI>();
            if (_textMesh != null)
            {
                _startColor = _textMesh.color;
            }

            _canvas = GetComponentInParent<Canvas>();
            if (_canvas != null)
            {
                _canvas.renderMode = RenderMode.WorldSpace;

                RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
                if (canvasRect != null)
                {
                    canvasRect.localPosition = Vector3.zero;
                    canvasRect.sizeDelta = new Vector2(200, 50);
                }

                _canvas.transform.localScale = Vector3.one * 0.01f;
            }
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            if (_timer > fadeStartTime && _textMesh != null)
            {
                float fadeProgress = (_timer - fadeStartTime) / (lifetime - fadeStartTime);
                Color color = _startColor;
                color.a = Mathf.Lerp(_startColor.a, 0f, fadeProgress);
                _textMesh.color = color;
            }

            if (_timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        public void SetText(string text)
        {
            if (_textMesh != null)
            {
                _textMesh.text = text;
            }
        }

        public void SetColor(Color color)
        {
            if (_textMesh != null)
            {
                _textMesh.color = color;
                _startColor = color;
            }
        }
    }
}
