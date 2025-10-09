using UnityEngine;
using UnityEngine.InputSystem;

public class VRMenuToggle : MonoBehaviour
{
    [Header("Références")]
    public GameObject menuCanvas;

    [Header("Input Action (Left Hand)")]
    public InputActionReference xButtonAction; 

    private void OnEnable()
    {
        xButtonAction.action.performed += OnXButtonPressed;
    }

    private void OnDisable()
    {
        xButtonAction.action.performed -= OnXButtonPressed;
    }

    private void OnXButtonPressed(InputAction.CallbackContext context)
    {
        if (menuCanvas == null) return;

        bool newState = !menuCanvas.activeSelf;
        menuCanvas.SetActive(newState);
    }

}
