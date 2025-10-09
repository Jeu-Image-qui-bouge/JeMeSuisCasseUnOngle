using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MagneticZone : MonoBehaviour
{
    [Header("Paramètres du champ magnétique")]
    public float attractionForce = 10f;
    public string targetTag = "Balle";
    public bool showGizmos = true;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(targetTag))
            return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        float distance = Vector3.Distance(transform.position, other.transform.position);

        if (distance <= 1f)
            return;

        Vector3 direction = (transform.position - other.transform.position).normalized;

        rb.AddForce(direction * attractionForce, ForceMode.Acceleration);
    }


    private void Reset()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 2f;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            SphereCollider col = GetComponent<SphereCollider>();
            float radius = (col != null) ? col.radius : 1f;
            Gizmos.color = new Color(0f, 0.6f, 1f, 0.3f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
