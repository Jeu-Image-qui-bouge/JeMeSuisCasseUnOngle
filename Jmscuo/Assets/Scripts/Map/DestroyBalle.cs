using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    [Header("Tag des objets � d�truire")]
    public string targetTag = "Balle";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Destroy(collision.gameObject);
        }
    }
}
