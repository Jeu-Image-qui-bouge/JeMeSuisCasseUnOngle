using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StickOnGround : MonoBehaviour
{
    [Header("Tag du sol")]
    public string groundTag = "Ground";

    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            rb.isKinematic = false;
        }
    }
}
