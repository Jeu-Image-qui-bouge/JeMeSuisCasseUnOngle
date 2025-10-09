using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ThrowableBall : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    [Header("Force du tir")]
    public float throwForce = 10f;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grabInteractable.activated.AddListener(OnActivate);
    }

    private void OnDestroy()
    {
        grabInteractable.activated.RemoveListener(OnActivate);
    }

    private void OnActivate(ActivateEventArgs args)
    {
        var interactor = args.interactorObject as IXRSelectInteractor;

        if (interactor == null)
            return;

        grabInteractable.interactionManager.SelectExit(interactor, grabInteractable);

        var controllerTransform = (interactor as Component)?.transform;
        if (controllerTransform == null)
            return;

        Vector3 direction = controllerTransform.forward;

        rb.isKinematic = false;
        rb.AddForce(direction * throwForce, ForceMode.VelocityChange);
    }
}
