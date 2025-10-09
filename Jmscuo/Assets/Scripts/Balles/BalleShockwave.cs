using UnityEngine;

public class ShockwaveOnCollision : MonoBehaviour
{
    [Header("Paramètres de l'impulsion")]
    public float radius = 1f;              
    public float force = 3f;                
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("Filtrage")]
    public string groundTag = "Ground";
    public string targetTag = "Balle";      
    public bool affectAll = false;            

    [Header("Debug")]
    public bool showGizmos = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(groundTag))
            return;

        Vector3 explosionPosition = collision.contacts[0].point;

        Collider[] colliders = Physics.OverlapSphere(explosionPosition, radius);

        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.attachedRigidbody;

            if (rb == null)
                continue;

            if (rb.gameObject == gameObject)
                continue;

            if (!affectAll && !nearby.CompareTag(targetTag))
                continue;

            Vector3 direction = (nearby.transform.position - explosionPosition).normalized;

            rb.AddForce(direction * force, forceMode);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
