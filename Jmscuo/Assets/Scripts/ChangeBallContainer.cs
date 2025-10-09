using UnityEngine;

public class ChangeTagOnTriggerExit : MonoBehaviour
{
    [Header("Filtrage")]
    public string targetTag = "NewBalle";
    public string newTag = "Balle";

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(targetTag))
            return;

        if (other != null)
        {
            other.tag = newTag;
        }
    }
}
