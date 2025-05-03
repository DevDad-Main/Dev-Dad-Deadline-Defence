using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InputSchemePrompt : MonoBehaviour
{
    [Header("Prompts")]
    [SerializeField] private GameObject keyboardPrompt;
    [SerializeField] private GameObject gamepadPrompt;

    private PlayerInput _playerInput;

    private void Start()
    {
        _playerInput = Player.Instance.PlayerInput;

        _playerInput.onControlsChanged += OnControlsChanged;
        UpdatePrompt(_playerInput.currentControlScheme);


    }

    private void OnDisable()
    {
        _playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        UpdatePrompt(input.currentControlScheme); 
    }

    private void UpdatePrompt(string controlScheme)
    {
        if (controlScheme.Contains("Gamepad"))
        {
            gamepadPrompt.SetActive(true);
            keyboardPrompt.SetActive(false);
        }
        else
        {
            gamepadPrompt.SetActive(false);
            keyboardPrompt.SetActive(true);
        }
    }
}
