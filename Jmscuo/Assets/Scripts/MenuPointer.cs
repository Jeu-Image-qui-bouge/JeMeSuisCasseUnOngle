using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRPointer : MonoBehaviour
{
    [Header("Références")]
    public XRRayInteractor rayInteractor;
    public InputActionReference triggerAction; 

    private void OnEnable()
    {
        triggerAction.action.performed += OnTriggerPressed;
    }

    private void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPressed;
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult result))
        {
            var button = result.gameObject.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
    }
}
