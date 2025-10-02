using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class MainMenuManager : MonoBehaviour
{
    private Button _playButton;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        _playButton = root.Q<Button>("play-button");

        if (_playButton != null)
        {
            _playButton.clicked += OnPlayButtonClicked;
        }
    }

    private void OnDisable()
    {
        if (_playButton != null)
        {
            _playButton.clicked -= OnPlayButtonClicked;
        }
    }

    private void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked! Loading game scene...");
        SceneManager.LoadScene("Main Level");
    }
}