using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StickOnGround : MonoBehaviour
{
    [Header("Tag du sol")]
    public string groundTag = "Ground";

    [Header("Haptique")]
    public bool hapticsOnImpact = true;
    [Range(0f, 1f)] public float hapticsAmplitude = 0.7f;
    [Range(0f, 1f)] public float hapticsDuration = 0.12f;
    public bool hapticsBothHands = true;

    private Rigidbody rb;
    private bool isOnGround = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isOnGround)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        /*else
        {
            isOnGround = false;
            rb.isKinematic = false;
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            isOnGround = true;
        }

        if (hapticsOnImpact)
        {
            if (hapticsBothHands) HapticPulseVR.PulseBoth(hapticsAmplitude, hapticsDuration);
            else HapticPulseVR.Pulse(false, hapticsAmplitude, hapticsDuration); // false = main droite (Right)
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            isOnGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            isOnGround = false;
            rb.isKinematic = false;
        }
    }
}
