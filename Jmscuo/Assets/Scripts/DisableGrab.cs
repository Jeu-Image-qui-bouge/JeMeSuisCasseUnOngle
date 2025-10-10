using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DisableGrabOnTriggerEnter : MonoBehaviour
{
    [Header("Filtrage")]
    public string targetTag = "Balle";

    [Header("Options")]
    public bool reenableOnExit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag))
            return;

        XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();

        if (grab != null && grab.enabled)
        {
            grab.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!reenableOnExit)
            return;

        if (!other.CompareTag(targetTag))
            return;

        XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();

        if (grab != null && !grab.enabled)
        {
            grab.enabled = true;
            Debug.Log($"{other.name}  Grab réactivé !");
        }
    }
}
